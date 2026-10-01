using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;
using System.Threading;

namespace BaseClinic.Business.Services.Appointments.Commands
{
    public record BookAppointmentCommand
    (
         Guid AccountId,
         Guid? DependentPatientId,
         string? NewDependentFullName,
         DateTime? NewDependentDob,
         PatientRelationshipType? NewDependentRelationship,
         Guid DepartmentId,
         Guid? RequestedDoctorId,
         DateTime AppointmentDate,
         TimeOnly StartTime,
         TimeOnly EndTime,
         string Reason
    ) : IRequest<Guid>;

    public class BookAppointmentCommandHander : IRequestHandler<BookAppointmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientDelegationRepository _patientDelegationRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientCodeGenerator _patientCodeGenerator;

        public BookAppointmentCommandHander(
            IUnitOfWork unitOfWork,
            IPatientRepository patientRepository,
            IPatientDelegationRepository patientDelegationRepository,
            IAppointmentRepository appointmentRepository,
            IPatientCodeGenerator patientCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _patientRepository = patientRepository;
            _patientDelegationRepository = patientDelegationRepository;
            _appointmentRepository = appointmentRepository;
            _patientCodeGenerator = patientCodeGenerator;
        }

        public async Task<Guid> Handle(BookAppointmentCommand request, CancellationToken cancellation)
        {
            await _unitOfWork.BeginTransactionAsync(cancellation);
            try
            {
                // Bước 1: Xác định hồ sơ chính (Dùng để đặt cho bản thân)
                var primaryPatient = await _patientRepository.GetPrimaryPatientByAccountIdAsync(request.AccountId, cancellation)
                    ?? throw new InvalidOperationException("Tài khoản chưa được liên kết với hồ sơ bệnh nhân gốc.");

                Guid targetPatientId = primaryPatient.Id;

                // Bước 2: Tự suy luận luồng đặt lịch
                if (request.DependentPatientId.HasValue)
                {
                    // Luồng A: Đặt cho hồ sơ phụ ĐÃ TỒN TẠI VÀ ĐƯỢC CẤP QUYỀN
                    // SỬA ĐỔI: Truyền request.AccountId thay vì primaryPatient.Id
                    var dependent = await _patientRepository.GetDependentPatientByIdAsync(request.DependentPatientId.Value, request.AccountId, cancellation)
                        ?? throw new InvalidOperationException("Hồ sơ phụ không tồn tại hoặc bạn chưa được cấp quyền xem hồ sơ này.");

                    targetPatientId = dependent.Id;
                }
                else if (!string.IsNullOrWhiteSpace(request.NewDependentFullName) && request.NewDependentRelationship.HasValue)
                {
                    // Luồng B: Khởi tạo hồ sơ phụ MỚI
                    string patientCode = await _patientCodeGenerator.GenerateAsync(cancellation);

                    var newDependent = new Patient(
                        patientCode: patientCode,
                        accountId: null,
                        fullName: request.NewDependentFullName,
                        isPrimary: false);

                    newDependent.UpdateProfile(
                        fullName: request.NewDependentFullName,
                        avatar: null,
                        fullAddress: null,
                        identityNumber: null,
                        gender: null,
                        dateOfBirth: request.NewDependentDob);

                    _patientRepository.Add(newDependent);

                    // Bắt buộc SaveChangesAsync để lấy ID của newDependent
                    await _unitOfWork.SaveChangesAsync(cancellation);

                    // Tạo giấy phép truy cập (PatientDelegation) cho người tạo
                    var delegation = new PatientDelegation(
                        targetPatientId: newDependent.Id,
                        observerAccountId: request.AccountId,
                        relationshipType: request.NewDependentRelationship.Value,
                        status: DelegationStatus.Accepted);

                    _patientDelegationRepository.AddDelegation(delegation);
                    await _unitOfWork.SaveChangesAsync(cancellation);

                    targetPatientId = newDependent.Id;
                }

                // Bước 3: Re-validate booking conditions
                // 3.1. Kiểm tra thời gian không được nằm trong quá khứ
                // (Giả định AppointmentDate lưu ngày, StartTime lưu giờ)
                var appointmentDateTime = request.AppointmentDate.Date.Add(request.StartTime.ToTimeSpan());
                if (appointmentDateTime < DateTime.Now)
                {
                    throw new InvalidOperationException("Không thể đặt lịch cho thời điểm trong quá khứ.");
                }

                // 3.2. Bệnh nhân không được phép có lịch khám khác trùng giờ (tránh double-booking)
                bool isPatientBusy = await _appointmentRepository.HasOverlappingAppointmentAsync(
                    patientId: targetPatientId,
                    date: request.AppointmentDate,
                    startTime: request.StartTime,
                    endTime: request.EndTime,
                    cancellationToken: cancellation);

                if (isPatientBusy)
                {
                    throw new InvalidOperationException("Hồ sơ bệnh nhân này đã có một lịch hẹn khác trong cùng khoảng thời gian.");
                }

                // 3.3. Nếu có chỉ định đích danh bác sĩ, kiểm tra xem bác sĩ có rảnh không
                if (request.RequestedDoctorId.HasValue)
                {
                    bool isDoctorBusy = await _appointmentRepository.HasOverlappingDoctorAppointmentAsync(
                        doctorId: request.RequestedDoctorId.Value,
                        date: request.AppointmentDate,
                        startTime: request.StartTime,
                        endTime: request.EndTime,
                        cancellationToken: cancellation);

                    if (isDoctorBusy)
                    {
                        throw new InvalidOperationException("Bác sĩ được yêu cầu đã kín lịch vào thời gian này. Vui lòng chọn khung giờ hoặc bác sĩ khác.");
                    }
                }

                // Bước 4: Tạo Appointment
                var appointment = new Appointment(
                    patientId: targetPatientId,
                    departmentId: request.DepartmentId,
                    appointmentDate: request.AppointmentDate,
                    startTime: request.StartTime,
                    endTime: request.EndTime,
                    reason: request.Reason,
                    requestedDoctorId: request.RequestedDoctorId);

                _appointmentRepository.Add(appointment);

                // Bước 5: Chốt giao dịch
                await _unitOfWork.CommitTransactionAsync(cancellation);

                return appointment.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellation);
                throw;
            }
        }
    }
}
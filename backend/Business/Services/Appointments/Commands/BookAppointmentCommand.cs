using BaseClinic.Business.Interfaces;
using BaseClinic.DataAccess.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;
using System.Threading;

namespace BaseClinic.Business.Services.Appointments.Commands
{
    public record BookAppointmentCommand
    (
         // 1. Định danh người thực hiện (Lấy từ JWT)
         Guid AccountId,

         // 2. Dữ liệu xác định bệnh nhân phụ (Nếu có)
         Guid? DependentPatientId,

         // 3. Dữ liệu tạo bệnh nhân phụ mới (Nếu có)
         string? NewDependentFullName,
         DateTime? NewDependentDob,
         PatientRelationshipType? NewDependentRelationship,

         // 4. Dữ liệu lịch hẹn
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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientCodeGenerator _patientCodeGenerator;

        public BookAppointmentCommandHander(
            IUnitOfWork unitOfWork,
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IPatientCodeGenerator patientCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _patientCodeGenerator = patientCodeGenerator;
        }

        public async Task<Guid> Handle(BookAppointmentCommand request, CancellationToken cancellation)
        {
            // Mở Transaction để đảm bảo tính toàn vẹn dữ liệu
            await _unitOfWork.BeginTransactionAsync(cancellation);
            try
            {
                //Bước 1: Khởi tạo/Xác định hồ sơ chính
                var primaryPatient = await _patientRepository.GetPrimaryPatientByAccountIdAsync(request.AccountId, cancellation)
                    ?? throw new InvalidOperationException("Tài khoản chưa được liên kết với hồ sơ bệnh nhân gốc.");

                Guid targetPatientId = primaryPatient.Id;

                // Bước 2: Tự suy luận luồng đặt lịch dựa vào dữ liệu đầu vào
                if (request.DependentPatientId.HasValue)
                {
                    // Luồng A: Đặt cho hồ sơ phụ đã tồn tại
                    var dependent = await _patientRepository.GetDependentPatientByIdAsync(request.DependentPatientId.Value, primaryPatient.Id, cancellation)
                        ?? throw new InvalidOperationException("Hồ sơ phụ không tồn tại hoặc không phụ thuộc quyền quản lý của bạn.");

                    targetPatientId = dependent.Id;
                }
                else if (!string.IsNullOrWhiteSpace(request.NewDependentFullName) && request.NewDependentRelationship.HasValue)
                {
                    // Luồng B: Khởi tạo hồ sơ phụ mới
                    string patientCode = await _patientCodeGenerator.GenerateAsync(cancellation);

                    // Sử dụng đúng Constructor của Patient (AccountId truyền vào bằng null)
                    var newDependent = new Patient(
                        patientCode: patientCode,
                        accountId: null,
                        fullName: request.NewDependentFullName,
                        primaryPatientId: primaryPatient.Id,
                        relationshipType: request.NewDependentRelationship);

                    // Gọi hàm UpdateProfile để cập nhật Ngày sinh (vì Constructor không nhận tham số này)
                    newDependent.UpdateProfile(
                        fullName: request.NewDependentFullName,
                        avatar: null,
                        fullAddress: null,
                        identityNumber: null,
                        gender: null,
                        dateOfBirth: request.NewDependentDob);

                    _patientRepository.Add(newDependent);

                    // Bắt buộc gọi SaveChangesAsync để EF Core cấp phát ID thật cho newDependent trước khi dùng
                    await _unitOfWork.SaveChangesAsync(cancellation);

                    targetPatientId = newDependent.Id;
                }
                // Nếu cả 2 IF trên đều sai, targetPatientId giữ nguyên là primaryPatient.Id (Luồng C: Tự đặt cho chính mình)

                // Bước 3: Re-validate booking conditions (Mock/Placeholder cho các bước 6, 9)
                // TODO: Truy vấn DepartmentRepository và DoctorScheduleRepository để kiểm tra giờ trống, sức chứa...

                // Bước 4: Tạo Appointment bằng Constructor chuẩn
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

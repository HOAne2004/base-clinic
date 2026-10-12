using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Domain.Extensions;
using MediatR;

namespace BaseClinic.Business.Services.Appointments.Commands
{
    /// <summary>
    /// Đặt lịch hẹn.
    /// 
    /// 3 LUỒNG:
    /// 1. Đặt cho chính mình (DependentPatientId = null, NewDependentFullName = null).
    /// 2. Đặt cho dependent đã tồn tại (DependentPatientId != null).
    /// 3. Đặt cho dependent mới (NewDependentFullName != null).
    /// </summary>
    public record BookAppointmentCommand(
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

    public class BookAppointmentCommandHandler
        : IRequestHandler<BookAppointmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IPatientDelegationRepository _delegationRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IPatientCodeGenerator _patientCodeGenerator;
        private readonly IAppointmentCodeGenerator _appointmentCodeGenerator;

        public BookAppointmentCommandHandler(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepo,
            IPatientRepository patientRepo,
            IPatientDelegationRepository delegationRepo,
            IAppointmentRepository appointmentRepo,
            IPatientCodeGenerator patientCodeGenerator,
            IAppointmentCodeGenerator appointmentCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _accountRepo = accountRepo;
            _patientRepo = patientRepo;
            _delegationRepo = delegationRepo;
            _appointmentRepo = appointmentRepo;
            _patientCodeGenerator = patientCodeGenerator;
            _appointmentCodeGenerator = appointmentCodeGenerator;
        }

        public async Task<Guid> Handle(
            BookAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Lấy thông tin người đặt ====================
            var account = await _accountRepo.GetByIdAsync(
                request.AccountId, cancellationToken)
                ?? throw new DomainException("Không tìm thấy tài khoản.");

            var primaryPatient = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken)
                ?? throw new DomainException(
                    "Tài khoản chưa được liên kết với hồ sơ bệnh nhân gốc.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                Guid targetPatientId = primaryPatient.Id;

                // ==================== BƯỚC 2: Xác định Patient target ====================
                if (request.DependentPatientId.HasValue)
                {
                    // LUỒNG 2: Đặt cho dependent đã tồn tại
                    var dependent = await _patientRepo.GetByIdAsync(
                        request.DependentPatientId.Value, cancellationToken)
                        ?? throw new DomainException("Không tìm thấy hồ sơ người phụ thuộc.");

                    // Check A có quyền với dependent này không
                    // - Hoặc dependent.PrimaryPatientId == primaryPatient.Id
                    // - Hoặc có delegation Accepted
                    bool hasAccess = dependent.PrimaryPatientId == primaryPatient.Id;

                    if (!hasAccess)
                    {
                        var delegation = await _delegationRepo
                            .GetActiveByTargetAndObserverAsync(
                                dependent.Id, request.AccountId, cancellationToken);
                        hasAccess = delegation != null;
                    }

                    if (!hasAccess)
                        throw new ForbiddenException(
                            "Bạn không có quyền đặt lịch cho hồ sơ này.");

                    targetPatientId = dependent.Id;
                }
                else if (!string.IsNullOrWhiteSpace(request.NewDependentFullName)
                         && request.NewDependentRelationship.HasValue)
                {
                    // LUỒNG 3: Tạo dependent mới
                    string patientCode = await _patientCodeGenerator
                        .GenerateAsync(cancellationToken);

                    var newDependent = new Patient(
                        patientCode: patientCode,
                        fullName: request.NewDependentFullName,
                        accountId: null,
                        primaryPatientId: primaryPatient.Id,
                        isPrimary: false,
                        relationshipType: request.NewDependentRelationship.Value);

                    newDependent.UpdateProfile(
                        fullName: request.NewDependentFullName,
                        avatarUrl: null,
                        address: null,
                        identityNumber: null,
                        gender: null,
                        dateOfBirth: request.NewDependentDob,
                        healthInsuranceNumber: null);

                    _patientRepo.Add(newDependent);

                    // Save để có Id
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Auto-delegation
                    var delegation = PatientDelegation.CreateOwnerDelegation(
                        targetPatientId: newDependent.Id,
                        ownerAccountId: request.AccountId,
                        relationshipType: request.NewDependentRelationship.Value.Invert());

                    _delegationRepo.Add(delegation);

                    targetPatientId = newDependent.Id;
                }

                // ==================== BƯỚC 3: Validate thời gian ====================
                var appointmentDateTime = request.AppointmentDate.Date
                    .Add(request.StartTime.ToTimeSpan());

                if (appointmentDateTime < DateTime.Now)
                    throw new DomainException("Không thể đặt lịch cho thời điểm trong quá khứ.");

                // ==================== BƯỚC 4: Check trùng lịch ====================
                bool isPatientBusy = await _appointmentRepo.HasOverlappingAppointmentAsync(
                    patientId: targetPatientId,
                    date: request.AppointmentDate,
                    startTime: request.StartTime,
                    endTime: request.EndTime,
                    cancellationToken: cancellationToken);

                if (isPatientBusy)
                    throw new DomainException(
                        "Hồ sơ bệnh nhân này đã có lịch hẹn trong cùng khoảng thời gian.");

                if (request.RequestedDoctorId.HasValue)
                {
                    bool isDoctorBusy = await _appointmentRepo
                        .HasOverlappingDoctorAppointmentAsync(
                            doctorId: request.RequestedDoctorId.Value,
                            date: request.AppointmentDate,
                            startTime: request.StartTime,
                            endTime: request.EndTime,
                            cancellationToken: cancellationToken);

                    if (isDoctorBusy)
                        throw new DomainException(
                            "Bác sĩ đã kín lịch vào thời gian này.");
                }

                // ==================== BƯỚC 5: Sinh AppointmentCode ====================
                var appointmentCode = await _appointmentCodeGenerator
                    .GenerateAsync(cancellationToken);

                // ==================== BƯỚC 6: Tạo Appointment ====================
                var appointment = new Appointment(
                    appointmentCode: appointmentCode,
                    patientId: targetPatientId,
                    contactPhone: account.PhoneNumber,           // ← Snapshot từ Account
                    appointmentDate: request.AppointmentDate,
                    startTime: request.StartTime,
                    endTime: request.EndTime,
                    departmentId: request.DepartmentId,
                    requestedDoctorId: request.RequestedDoctorId,
                    contactName: null,                           // Fallback = Patient.FullName
                    contactRelationship: null,
                    reason: request.Reason,
                    isFromReceptionist: false);                  // User tự đặt → Pending

                _appointmentRepo.Add(appointment);

                // ==================== BƯỚC 7: Commit ====================
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return appointment.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
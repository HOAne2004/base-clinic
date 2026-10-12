using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Appointments.Commands
{
    public record CheckInCommand(
        Guid ReceptionistId,
        Guid? AppointmentId,
        Guid? PatientId,
        Guid? DepartmentId,
        string? NewPatientFullName,
        DateTime? NewPatientDob,
        bool IsPriority,
        QueueType Type
    ) : IRequest<CheckInResultDto>;

    public class CheckInCommandHandler : IRequestHandler<CheckInCommand, CheckInResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IQueueRepository _queueRepo;
        private readonly IEncounterRepository _encounterRepo;
        private readonly IPatientCodeGenerator _patientCodeGenerator;

        public CheckInCommandHandler(
            IUnitOfWork unitOfWork,
            IAppointmentRepository appointmentRepo,
            IDepartmentRepository departmentRepo,
            IPatientRepository patientRepo,
            IQueueRepository queueRepo,
            IEncounterRepository encounterRepo,
            IPatientCodeGenerator patientCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _appointmentRepo = appointmentRepo;
            _departmentRepo = departmentRepo;
            _patientRepo = patientRepo;
            _queueRepo = queueRepo;
            _encounterRepo = encounterRepo;
            _patientCodeGenerator = patientCodeGenerator;
        }

        public async Task<CheckInResultDto> Handle(
            CheckInCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var checkInTime = DateTime.UtcNow;
                Guid targetPatientId;
                Guid targetDepartmentId;
                Guid? linkedAppointmentId = null;
                string targetPatientName = string.Empty;

                // ==================== BƯỚC 1: XÁC ĐỊNH PATIENT + KHOA ====================
                if (request.AppointmentId.HasValue)
                {
                    // Flow chính: Check-in từ Appointment
                    var appointment = await _appointmentRepo.GetByIdAsync(
                        request.AppointmentId.Value, cancellationToken)
                        ?? throw new NotFoundException("Không tìm thấy lịch hẹn.");

                    if (appointment.Status != AppointmentStatus.Confirmed)
                        throw new DomainException(
                            $"Không thể check-in. Trạng thái lịch hẹn hiện tại: {appointment.Status}");

                    targetPatientId = appointment.PatientId;
                    targetDepartmentId = appointment.DepartmentId
                        ?? throw new DomainException("Lịch hẹn chưa được phân khoa.");
                    linkedAppointmentId = appointment.Id;

                    var patient = await _patientRepo.GetByIdAsync(
                        targetPatientId, cancellationToken);
                    targetPatientName = patient?.FullName ?? string.Empty;

                    appointment.CheckIn();
                }
                else
                {
                    // Alternative Flow A & C: Walk-in
                    targetDepartmentId = request.DepartmentId
                        ?? throw new DomainException(
                            "Walk-in bắt buộc phải chọn khoa (DepartmentId).");

                    if (request.PatientId.HasValue)
                    {
                        // Walk-in cho bệnh nhân cũ
                        var patient = await _patientRepo.GetByIdAsync(
                            request.PatientId.Value, cancellationToken)
                            ?? throw new NotFoundException("Không tìm thấy hồ sơ bệnh nhân cũ.");

                        targetPatientId = patient.Id;
                        targetPatientName = patient.FullName;
                    }
                    else if (!string.IsNullOrWhiteSpace(request.NewPatientFullName)
                             && request.NewPatientDob.HasValue)
                    {
                        // Walk-in chưa có hồ sơ → tạo mới
                        string patientCode = await _patientCodeGenerator
                            .GenerateAsync(cancellationToken);

                        // ⚡ Fix: Constructor mới của Patient
                        //    Signature: (patientCode, fullName, accountId?, primaryPatientId?, isPrimary, relationshipType?)
                        var newPatient = new Patient(
                            patientCode: patientCode,
                            fullName: request.NewPatientFullName,
                            accountId: null,
                            primaryPatientId: null,
                            isPrimary: true);

                        // ⚡ Fix: UpdateProfile signature mới
                        //    Signature: (fullName, avatarUrl, address, identityNumber, gender, dob, healthInsuranceNumber?)
                        newPatient.UpdateProfile(
                            fullName: request.NewPatientFullName,
                            avatarUrl: null,
                            address: null,
                            identityNumber: null,
                            gender: null,
                            dateOfBirth: request.NewPatientDob,
                            healthInsuranceNumber: null);

                        _patientRepo.Add(newPatient);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        targetPatientId = newPatient.Id;
                        targetPatientName = request.NewPatientFullName;
                    }
                    else
                    {
                        throw new DomainException(
                            "Bắt buộc phải cung cấp ID bệnh nhân cũ " +
                            "hoặc thông tin khởi tạo hồ sơ mới.");
                    }
                }

                // ==================== BƯỚC 2: TẠO ENCOUNTER ====================
                string encounterCode = await _encounterRepo
                    .GenerateEncounterCodeAsync(cancellationToken);

                var encounter = new Encounter(
                    encounterCode: encounterCode,
                    patientId: targetPatientId,
                    appointmentId: linkedAppointmentId ?? Guid.Empty,
                    checkInAt: checkInTime);

                _encounterRepo.Add(encounter);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // ==================== BƯỚC 3: ĐƯA VÀO HÀNG ĐỢI ====================
                var queue = await _queueRepo.GetActiveQueueAsync(
                    targetDepartmentId, checkInTime.Date, cancellationToken);

                if (queue == null)
                {
                    string queueCode = await _queueRepo.GenerateQueueCodeAsync(
                        targetDepartmentId, checkInTime.Date, cancellationToken);

                    // Queue chưa refactor → giữ constructor cũ
                    queue = new Queue(
                        targetDepartmentId,
                        queueCode,
                        checkInTime.Date,
                        request.Type);

                    _queueRepo.Add(queue);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                var queueEntry = queue.Enqueue(
                    encounter.Id, request.IsPriority, checkInTime);

                // ==================== BƯỚC 4: COMMIT ====================
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // ==================== BƯỚC 5: LẤY TÊN KHOA ====================
                var department = await _departmentRepo.GetByIdAsync(
                    targetDepartmentId, cancellationToken);

                string departmentName = department?.Name ?? "Khoa không xác định";

                // ==================== BƯỚC 6: RESPONSE ====================
                return new CheckInResultDto(
                    encounter.Id,
                    encounter.EncounterCode,
                    queue.Id,
                    queueEntry.QueueNumber,
                    departmentName,
                    targetPatientName,
                    checkInTime);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
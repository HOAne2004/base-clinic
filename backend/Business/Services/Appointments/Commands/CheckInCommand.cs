using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;

namespace BaseClinic.Business.Services.Appointments.Commands
{
    public record CheckInCommand
        (
            // Định danh người thao tác (Receptionist) lấy từ Token
            Guid ReceptionistId,
            // --- Trường hợp 1: Check-in từ lịch hẹn có sẵn ---
            Guid? AppointmentId,
            // --- Trường hợp 2: Walk-in (Không có lịch hẹn) ---
            // Nếu bệnh nhân cũ
            Guid? PatientId,
            Guid? DepartmentId,
            // Nếu Walk-in mà bệnh nhân chưa từng tồn tại (Alternative C)
            string? NewPatientFullName,
            DateTime? NewPatientDob,
            bool IsPriority,
            QueueType Type
        ) : IRequest<CheckInResultDto>;

    public class CheckInCommandHandler : IRequestHandler<CheckInCommand, CheckInResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IQueueRepository _queueRepository;
        private readonly IEncounterRepository _encounterRepository;
        private readonly IPatientCodeGenerator _patientCodeGenerator;

        public CheckInCommandHandler(
            IUnitOfWork unitOfWork,
            IAppointmentRepository appointmentRepository,
            IDepartmentRepository departmentRepository,
            IPatientRepository patientRepository,
            IQueueRepository queueRepository,
            IEncounterRepository encounterRepository,
            IPatientCodeGenerator patientCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _appointmentRepository = appointmentRepository;
            _departmentRepository = departmentRepository;
            _patientRepository = patientRepository;
            _queueRepository = queueRepository;
            _encounterRepository = encounterRepository;
            _patientCodeGenerator = patientCodeGenerator;
        }

        public async Task<CheckInResultDto> Handle(CheckInCommand request, CancellationToken cancellationToken)
        {
            // BR-13: Đảm bảo toàn vẹn dữ liệu (Encounter, QueueEntry, Appointment Update)
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var checkInTime = DateTime.UtcNow;
                Guid targetPatientId;
                Guid targetDepartmentId;
                Guid? linkedAppointmentId = null;
                string targetPatientName = string.Empty;

                // --- BƯỚC 1: XÁC ĐỊNH BỆNH NHÂN & KHOA (BR-02, BR-05) ---
                if (request.AppointmentId.HasValue)
                {
                    // Flow Chính: Check-in từ Appointment
                    var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId.Value, cancellationToken)
                        ?? throw new InvalidOperationException("Không tìm thấy lịch hẹn.");

                    // BR-03: Kiểm tra trạng thái hợp lệ
                    if (appointment.Status != AppointmentStatus.Confirmed)
                        throw new InvalidOperationException($"Không thể check-in. Trạng thái lịch hẹn hiện tại: {appointment.Status}");

                    targetPatientId = appointment.PatientId;
                    targetDepartmentId = appointment.DepartmentId ?? throw new InvalidOperationException("Lịch hẹn chưa được phân khoa.");
                    linkedAppointmentId = appointment.Id;

                    // Lấy tên bệnh nhân (Để trả về DTO)
                    var patient = await _patientRepository.GetByIdAsync(targetPatientId, cancellationToken);
                    targetPatientName = patient?.FullName ?? string.Empty;

                    // BR-12: Chuyển trạng thái Appointment sang CheckedIn
                    appointment.CheckIn();
                }
                else
                {
                    // Alternative Flow A & C: Walk-in
                    targetDepartmentId = request.DepartmentId ?? throw new ArgumentException("Walk-in bắt buộc phải chọn khoa (DepartmentId).");

                    if (request.PatientId.HasValue)
                    {
                        // Walk-in cho bệnh nhân cũ
                        var patient = await _patientRepository.GetByIdAsync(request.PatientId.Value, cancellationToken)
                            ?? throw new InvalidOperationException("Không tìm thấy hồ sơ bệnh nhân cũ.");
                        targetPatientId = patient.Id;
                        targetPatientName = patient.FullName ?? string.Empty;
                    }
                    else if (!string.IsNullOrWhiteSpace(request.NewPatientFullName) && request.NewPatientDob.HasValue)
                    {
                        // Walk-in nhưng chưa có hồ sơ (Tạo mới)
                        string patientCode = await _patientCodeGenerator.GenerateAsync(cancellationToken);
                        var newPatient = new Patient(patientCode, null, request.NewPatientFullName);
                        newPatient.UpdateProfile(request.NewPatientFullName, null, null, null, null, request.NewPatientDob);

                        _patientRepository.Add(newPatient);
                        await _unitOfWork.SaveChangesAsync(cancellationToken); // Lấy ID mới

                        targetPatientId = newPatient.Id;
                        targetPatientName = request.NewPatientFullName;
                    }
                    else
                    {
                        throw new ArgumentException("Bắt buộc phải cung cấp ID bệnh nhân cũ hoặc thông tin khởi tạo hồ sơ mới.");
                    }
                }

                // --- BƯỚC 2: TẠO ENCOUNTER (BR-04, BR-06) ---
                string encounterCode = await _encounterRepository.GenerateEncounterCodeAsync(cancellationToken);

                // Khởi tạo Encounter (Nếu là Walk-in thì truyền Guid.Empty vào AppointmentId do constructor không cho null)
                var encounter = new Encounter(
                    encounterCode: encounterCode,
                    patientId: targetPatientId,
                    appointmentId: linkedAppointmentId ?? Guid.Empty,
                    checkInAt: checkInTime);

                _encounterRepository.Add(encounter);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // Lấy Encounter ID

                // --- BƯỚC 3: ĐƯA VÀO HÀNG ĐỢI (BR-07, BR-08, BR-09, BR-10, BR-11) ---
                // Tìm Queue của khoa trong ngày hôm nay. Nếu chưa có thì tự động mở Queue mới.
                var queue = await _queueRepository.GetActiveQueueAsync(targetDepartmentId, checkInTime.Date, cancellationToken);
                if (queue == null)
                {
                    string queueCode = await _queueRepository.GenerateQueueCodeAsync(targetDepartmentId, checkInTime.Date, cancellationToken);
                    queue = new Queue(targetDepartmentId, queueCode, checkInTime.Date, request.Type);
                    _queueRepository.Add(queue);
                    await _unitOfWork.SaveChangesAsync(cancellationToken); // Save để có Queue ID
                }

                // Gọi hàm Enqueue bên trong Aggregate Root Queue (Đảm bảo số thứ tự được cấp đúng BR-09)
                var queueEntry = queue.Enqueue(encounter.Id, request.IsPriority, checkInTime);

                // --- BƯỚC 4: HOÀN TẤT TRANSACTION ---
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // --- BƯỚC 5: LẤY TÊN KHOA VÀ TRẢ KẾT QUẢ ---
                var department = await _departmentRepository.GetByIdAsync(targetDepartmentId, cancellationToken);
                string departmentName = department?.Name ?? "Khoa không xác định";

                // --- TRẢ KẾT QUẢ CHO LỄ TÂN ---
                // Sử dụng cú pháp Positional Record (Ngoặc tròn) để sửa lỗi CS7036
                return new CheckInResultDto(
                    encounter.Id,                 // 1. EncounterId
                    encounter.EncounterCode,      // 2. EncounterCode
                    queue.Id,                     // 3. QueueId
                    queueEntry.QueueNumber,       // 4. QueueNumber
                    departmentName,               // 5. DepartmentName
                    targetPatientName,            // 6. PatientName
                    checkInTime                   // 7. CheckInTime
                );
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }

}

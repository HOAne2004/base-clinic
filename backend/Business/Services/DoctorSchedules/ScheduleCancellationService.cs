using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules
{
    public class ScheduleCancellationService : IScheduleCancellationService
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ISmsService _smsService;

        public ScheduleCancellationService(
            IDoctorScheduleRepository scheduleRepo,
            IAppointmentRepository appointmentRepo,
            IAccountRepository accountRepo,
            ISmsService smsService)
        {
            _scheduleRepo = scheduleRepo;
            _appointmentRepo = appointmentRepo;
            _accountRepo = accountRepo;
            _smsService = smsService;
        }

        public async Task<CancelScheduleResultDto> CancelAsync(
            Guid scheduleId,
            Guid cancelledByAccountId,
            string reason,
            CancellationToken cancellationToken = default)
        {
            // ==================== BƯỚC 1: Load schedule + items + doctor ====================
            var schedule = await _scheduleRepo.GetByIdWithItemsAsync(
                scheduleId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            if (schedule.Status == DoctorScheduleStatus.Cancelled)
                throw new DomainException("Ca này đã được hủy trước đó.");

            if (schedule.Status == DoctorScheduleStatus.Completed)
                throw new DomainException("Không thể hủy ca đã hoàn thành.");

            // ==================== BƯỚC 2: Load appointments trong slots của ca ====================
            var slotIds = schedule.Items.Select(i => i.Id).ToList();

            var appointments = await _appointmentRepo.GetQueryableWithTracking()
                .Where(a => a.DoctorScheduleItemId.HasValue
                         && slotIds.Contains(a.DoctorScheduleItemId.Value)
                         && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync(cancellationToken);

            // ==================== BƯỚC 3: Check blocking ====================
            var blockingAppointments = appointments
                .Where(a => a.Status == AppointmentStatus.CheckedIn
                         || a.Status == AppointmentStatus.Completed
                         || a.Status == AppointmentStatus.NoShow)
                .ToList();

            if (blockingAppointments.Any())
                throw new DomainException(
                    $"Không thể hủy ca vì có {blockingAppointments.Count} bệnh nhân " +
                    $"đã check-in hoặc đã hoàn thành. Vui lòng xử lý các appointment trước.");

            // ==================== BƯỚC 4: Hủy appointments + release slots ====================
            var cancelledAppointmentIds = new List<Guid>();
            var releasedSlots = 0;

            var cancelReason = $"Ca làm việc bị hủy. Lý do: {reason}";

            foreach (var appt in appointments)
            {
                // Cancel appointment (đã check ở BƯỚC 3 → chắc chắn pass)
                appt.Cancel(cancelReason);
                cancelledAppointmentIds.Add(appt.Id);

                // Release slot tương ứng
                if (appt.DoctorScheduleItemId.HasValue)
                {
                    var slot = schedule.Items
                        .FirstOrDefault(i => i.Id == appt.DoctorScheduleItemId.Value);

                    if (slot != null)
                    {
                        slot.ReleaseOne();
                        releasedSlots++;
                    }
                }
            }

            // ==================== BƯỚC 5: Hủy schedule ====================
            schedule.Cancel(cancelledByAccountId, reason);

            // ==================== BƯỚC 6: SMS cho bệnh nhân ====================
            await SendNotificationsAsync(
                schedule, appointments, reason, cancellationToken);

            // ==================== BƯỚC 7: Return (KHÔNG SaveChanges) ====================
            return new CancelScheduleResultDto(
                CancelledAppointments: cancelledAppointmentIds.Count,
                ReleasedSlots: releasedSlots,
                CancelledAppointmentIds: cancelledAppointmentIds);
        }

        // ==================== PRIVATE HELPERS ====================

        private async Task SendNotificationsAsync(
            Domain.Entities.DoctorSchedule schedule,
            List<Appointment> appointments,
            string reason,
            CancellationToken cancellationToken)
        {
            if (!appointments.Any())
                return;

            // Lấy tên bác sĩ
            var doctorAccount = await _accountRepo.GetByIdAsync(
                schedule.Doctor.AccountId, cancellationToken);
            var doctorName = doctorAccount?.FullName ?? "bác sĩ";

            // Format thời gian
            var dateStr = schedule.WorkDate.ToString("dd/MM/yyyy");

            // Gửi SMS cho từng appointment (dùng ContactPhone snapshot)
            foreach (var appt in appointments)
            {
                if (string.IsNullOrWhiteSpace(appt.ContactPhone))
                    continue;

                var message = $"[BaseClinic] Ca khám với {doctorName} ngày {dateStr} " +
                              $"({appt.StartTime:HH\\:mm}-{appt.EndTime:HH\\:mm}) đã bị hủy. " +
                              $"Lý do: {reason}. " +
                              $"Vui lòng liên hệ hotline để đặt lịch mới.";

                await _smsService.SendNotificationAsync(
                    phoneNumber: appt.ContactPhone,
                    message: message,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Đánh dấu ca làm việc đã hoàn thành.
    /// 
    /// ĐIỀU KIỆN:
    /// - Ca đã kết thúc về mặt thời gian (WorkDate + EndTime <= now).
    /// - Status hiện tại không phải Cancelled/Completed.
    /// </summary>
    public record CompleteScheduleCommand(Guid Id) : IRequest<bool>;

    public class CompleteScheduleCommandHandler
        : IRequestHandler<CompleteScheduleCommand, bool>
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteScheduleCommandHandler(
            IDoctorScheduleRepository scheduleRepo,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepo = scheduleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            CompleteScheduleCommand request,
            CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(
                request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            // Check đã qua thời gian kết thúc chưa
            var now = DateTime.UtcNow;
            var scheduleEnd = schedule.WorkDate
                .ToDateTime(schedule.EndTime)
                .Add(TimeSpan.FromHours(7)); // VN là UTC+7 → giờ local

            // So sánh: giờ VN của schedule so với giờ UTC hiện tại
            // Đơn giản hơn: dùng DateTime.UtcNow và chỉ so DateOnly + TimeOnly
            var today = DateOnly.FromDateTime(now);
            var nowTime = TimeOnly.FromDateTime(now);

            bool scheduleEnded = schedule.WorkDate < today
                || (schedule.WorkDate == today && schedule.EndTime <= nowTime);

            if (!scheduleEnded)
                throw new DomainException(
                    "Chưa đến thời gian kết thúc ca. Không thể đánh dấu hoàn thành.");

            schedule.Complete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
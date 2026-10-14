using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Bật/tắt đặt lịch trực tuyến cho 1 ca cụ thể.
    /// 
    /// LƯU Ý:
    /// - Không ảnh hưởng đến các appointment đã đặt.
    /// - Chỉ ngăn các booking MỚI.
    /// </summary>
    public record SetScheduleOnlineBookingCommand(
        Guid Id,
        bool Enabled
    ) : IRequest<bool>;

    public class SetScheduleOnlineBookingCommandHandler
        : IRequestHandler<SetScheduleOnlineBookingCommand, bool>
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public SetScheduleOnlineBookingCommandHandler(
            IDoctorScheduleRepository scheduleRepo,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepo = scheduleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            SetScheduleOnlineBookingCommand request,
            CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(
                request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            if (schedule.Status is DoctorScheduleStatus.Completed or DoctorScheduleStatus.Cancelled)
                throw new DomainException(
                    "Không thể đổi trạng thái ca đã kết thúc.");

            // Idempotent
            if (schedule.OnlineBookingEnabled == request.Enabled)
                return true;

            schedule.SetOnlineBookingEnabled(request.Enabled);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Xóa mềm ca làm việc.
    /// 
    /// ĐIỀU KIỆN:
    /// - Chưa có slot nào bị book.
    /// - Không ở trạng thái Completed.
    /// 
    /// Nếu muốn hủy ca đã có booking → dùng CancelScheduleCommand.
    /// </summary>
    public record DeleteScheduleCommand(Guid Id) : IRequest<bool>;

    public class DeleteScheduleCommandHandler
        : IRequestHandler<DeleteScheduleCommand, bool>
    {
        private readonly IDoctorScheduleRepository _scheduleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteScheduleCommandHandler(
            IDoctorScheduleRepository scheduleRepo,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepo = scheduleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteScheduleCommand request,
            CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepo.GetByIdWithItemsAsync(
                request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc.");

            if (schedule.Status == DoctorScheduleStatus.Completed)
                throw new DomainException(
                    "Không thể xóa ca đã hoàn thành.");

            // Check có slot nào bị book không
            if (schedule.Items.Any(i => i.BookedCount > 0))
                throw new DomainException(
                    "Không thể xóa ca đã có bệnh nhân đặt slot. " +
                    "Vui lòng dùng chức năng hủy ca.");

            _scheduleRepo.Delete(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
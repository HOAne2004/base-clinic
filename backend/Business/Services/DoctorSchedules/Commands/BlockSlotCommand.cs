using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Khóa 1 slot cụ thể — không cho bệnh nhân đặt.
    /// 
    /// DÙNG KHI:
    /// - Dành slot cho bệnh nhân tái khám.
    /// - Bác sĩ có việc đột xuất trong 1 slot.
    /// </summary>
    public record BlockSlotCommand(Guid SlotId) : IRequest<bool>;

    public class BlockSlotCommandHandler
        : IRequestHandler<BlockSlotCommand, bool>
    {
        private readonly IRepository<DoctorScheduleItem> _itemRepo;
        private readonly IUnitOfWork _unitOfWork;

        public BlockSlotCommandHandler(
            IRepository<DoctorScheduleItem> itemRepo,
            IUnitOfWork unitOfWork)
        {
            _itemRepo = itemRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            BlockSlotCommand request,
            CancellationToken cancellationToken)
        {
            var slot = await _itemRepo.GetByIdAsync(request.SlotId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy slot.");

            slot.Block();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
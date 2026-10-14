using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Mở khóa slot.
    /// </summary>
    public record UnblockSlotCommand(Guid SlotId) : IRequest<bool>;

    public class UnblockSlotCommandHandler
        : IRequestHandler<UnblockSlotCommand, bool>
    {
        private readonly IRepository<DoctorScheduleItem> _itemRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UnblockSlotCommandHandler(
            IRepository<DoctorScheduleItem> itemRepo,
            IUnitOfWork unitOfWork)
        {
            _itemRepo = itemRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UnblockSlotCommand request,
            CancellationToken cancellationToken)
        {
            var slot = await _itemRepo.GetByIdAsync(request.SlotId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy slot.");

            slot.Unblock();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
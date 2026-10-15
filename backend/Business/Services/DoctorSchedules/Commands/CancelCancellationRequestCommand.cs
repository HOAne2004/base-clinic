using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedules.Commands
{
    /// <summary>
    /// Requester tự hủy yêu cầu hủy ca (chỉ khi Pending).
    /// </summary>
    public record CancelCancellationRequestCommand(
        Guid RequestId,
        Guid RequesterAccountId
    ) : IRequest<bool>;

    public class CancelCancellationRequestCommandHandler
        : IRequestHandler<CancelCancellationRequestCommand, bool>
    {
        private readonly IRepository<ScheduleCancellationRequest> _requestRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CancelCancellationRequestCommandHandler(
            IRepository<ScheduleCancellationRequest> requestRepo,
            IUnitOfWork unitOfWork)
        {
            _requestRepo = requestRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            CancelCancellationRequestCommand request,
            CancellationToken cancellationToken)
        {
            var cancelRequest = await _requestRepo.GetByIdAsync(
                request.RequestId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy yêu cầu hủy.");

            // Domain method tự check ownership + status
            cancelRequest.CancelByRequester(request.RequesterAccountId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    public record RejectMergeRollbackCommand(
        Guid RequestId,
        Guid ReviewerAccountId,
        string ReviewNote
    ) : IRequest<Unit>;

    public class RejectMergeRollbackCommandHandler
        : IRequestHandler<RejectMergeRollbackCommand, Unit>
    {
        private readonly IRepository<MergeRollbackRequest> _requestRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;

        public RejectMergeRollbackCommandHandler(
            IRepository<MergeRollbackRequest> requestRepo,
            IAccountRepository accountRepo,
            ISmsService smsService,
            IUnitOfWork unitOfWork)
        {
            _requestRepo = requestRepo;
            _accountRepo = accountRepo;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            RejectMergeRollbackCommand request,
            CancellationToken cancellationToken)
        {
            var rollbackRequest = await _requestRepo.GetByIdAsync(
                request.RequestId, cancellationToken);

            if (rollbackRequest == null)
                throw new NotFoundException("Không tìm thấy yêu cầu.");

            rollbackRequest.Reject(request.ReviewerAccountId, request.ReviewNote);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // SMS cho requester
            var requester = await _accountRepo.GetByIdAsync(
                rollbackRequest.RequestedByAccountId, cancellationToken);

            if (requester != null)
            {
                await _smsService.SendNotificationAsync(
                    requester.PhoneNumber,
                    $"[BaseClinic] Yêu cầu hoàn tác gộp hồ sơ đã bị từ chối. " +
                    $"Lý do: {request.ReviewNote}",
                    cancellationToken);
            }

            return Unit.Value;
        }
    }
}
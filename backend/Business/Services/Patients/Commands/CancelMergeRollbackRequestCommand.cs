using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    public record CancelMergeRollbackRequestCommand(
        Guid RequestId,
        Guid RequesterAccountId
    ) : IRequest<Unit>;

    public class CancelMergeRollbackRequestCommandHandler
        : IRequestHandler<CancelMergeRollbackRequestCommand, Unit>
    {
        private readonly IRepository<MergeRollbackRequest> _requestRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CancelMergeRollbackRequestCommandHandler(
            IRepository<MergeRollbackRequest> requestRepo,
            IUnitOfWork unitOfWork)
        {
            _requestRepo = requestRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            CancelMergeRollbackRequestCommand request,
            CancellationToken cancellationToken)
        {
            var rollbackRequest = await _requestRepo.GetByIdAsync(
                request.RequestId, cancellationToken);

            if (rollbackRequest == null)
                throw new NotFoundException("Không tìm thấy yêu cầu.");

            // Domain method tự check ownership + status
            rollbackRequest.Cancel(request.RequesterAccountId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
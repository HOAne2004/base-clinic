using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Patients.Queries
{
    public record GetPendingRollbackRequestsQuery()
        : IRequest<List<MergeRollbackRequestDto>>;

    public class GetPendingRollbackRequestsQueryHandler
        : IRequestHandler<GetPendingRollbackRequestsQuery, List<MergeRollbackRequestDto>>
    {
        private readonly IRepository<MergeRollbackRequest> _requestRepo;

        public GetPendingRollbackRequestsQueryHandler(
            IRepository<MergeRollbackRequest> requestRepo)
        {
            _requestRepo = requestRepo;
        }

        public async Task<List<MergeRollbackRequestDto>> Handle(
            GetPendingRollbackRequestsQuery request,
            CancellationToken cancellationToken)
        {
            return await _requestRepo.GetQueryable()
                .Where(r => r.Status == MergeRollbackRequestStatus.Pending)
                .OrderBy(r => r.CreatedDate)
                .Select(r => new MergeRollbackRequestDto(
                    r.Id,
                    r.PatientMergeLogId,
                    r.RequestedByAccount.FullName,
                    r.Reason,
                    r.Status,
                    r.CreatedDate,
                    null,
                    null,
                    "",
                    ""
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
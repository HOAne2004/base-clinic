using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Patients.Queries
{
    public record GetMyRollbackRequestsQuery(Guid AccountId)
        : IRequest<List<MergeRollbackRequestDto>>;

    public class GetMyRollbackRequestsQueryHandler
        : IRequestHandler<GetMyRollbackRequestsQuery, List<MergeRollbackRequestDto>>
    {
        private readonly IRepository<MergeRollbackRequest> _requestRepo;

        public GetMyRollbackRequestsQueryHandler(
            IRepository<MergeRollbackRequest> requestRepo)
        {
            _requestRepo = requestRepo;
        }

        public async Task<List<MergeRollbackRequestDto>> Handle(
            GetMyRollbackRequestsQuery request,
            CancellationToken cancellationToken)
        {
            return await _requestRepo.GetQueryable()
                .Where(r => r.RequestedByAccountId == request.AccountId)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new MergeRollbackRequestDto(
                    r.Id,
                    r.PatientMergeLogId,
                    r.RequestedByAccount.FullName,
                    r.Reason,
                    r.Status,
                    r.CreatedDate,
                    r.ReviewedAt,
                    r.ReviewNote,
                    "", // SurvivorPatientCode — cần join
                    ""  // MergedPatientCode — cần join
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
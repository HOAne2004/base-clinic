using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Patient;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Patients.Queries
{
    public record GetPatientsQuery(PatientSearchDto Search)
        : IRequest<PagedResultDto<PatientListItemDto>>;

    public class GetPatientsQueryHandler
        : IRequestHandler<GetPatientsQuery, PagedResultDto<PatientListItemDto>>
    {
        private readonly IPatientRepository _patientRepo;

        public GetPatientsQueryHandler(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public async Task<PagedResultDto<PatientListItemDto>> Handle(
            GetPatientsQuery request,
            CancellationToken cancellationToken)
        {
            var search = request.Search;
            var query = _patientRepo.GetQueryableWithAccount();

            // KEYWORD: OR across fields
            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.Trim();
                query = query.Where(p =>
                    p.PatientCode.Contains(kw) ||
                    p.FullName.Contains(kw) ||
                    (p.IdentityNumber != null && p.IdentityNumber.Contains(kw)) ||
                    (p.HealthInsuranceNumber != null && p.HealthInsuranceNumber.Contains(kw)) ||
                    (p.Account != null && p.Account.PhoneNumber.Contains(kw)));
            }

            // FILTERS: AND
            if (search.Gender.HasValue)
                query = query.Where(p => p.Gender == search.Gender.Value);

            if (search.IsPrimary.HasValue)
                query = query.Where(p => p.IsPrimary == search.IsPrimary.Value);

            if (search.FromDate.HasValue)
                query = query.Where(p => p.CreatedDate >= search.FromDate.Value);

            if (search.ToDate.HasValue)
                query = query.Where(p => p.CreatedDate <= search.ToDate.Value);

            // PAGING
            var total = await query.CountAsync(cancellationToken);

            var entities = await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((search.PageIndex - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync(cancellationToken);

            // Sau khi lấy entities ra khỏi DB:
            var items = entities.Select(PatientMapper.ToListItemDto).ToList();

            return new PagedResultDto<PatientListItemDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = search.PageIndex,
                PageSize = search.PageSize
            };
        }
    }
}
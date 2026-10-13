using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Doctor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    /// <summary>
    /// Danh sách bác sĩ cho Admin/Receptionist.
    /// Có filter + paging + IncludeDeleted.
    /// 
    /// SORT: SortOrder ASC → CreatedDate DESC.
    /// </summary>
    public record GetAllDoctorsQuery(DoctorSearchDto Search)
        : IRequest<PagedResultDto<DoctorAdminListDto>>;

    public class GetAllDoctorsQueryHandler
        : IRequestHandler<GetAllDoctorsQuery, PagedResultDto<DoctorAdminListDto>>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetAllDoctorsQueryHandler(
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<PagedResultDto<DoctorAdminListDto>> Handle(
            GetAllDoctorsQuery request,
            CancellationToken cancellationToken)
        {
            var search = request.Search;

            // ==================== BƯỚC 1: Query doctors ====================
            var query = search.IncludeDeleted
                ? _doctorRepo.GetQueryable().IgnoreQueryFilters()
                : _doctorRepo.GetQueryable();

            if (search.DepartmentId.HasValue)
                query = query.Where(d => d.DepartmentId == search.DepartmentId.Value);

            if (search.IsActive.HasValue)
                query = query.Where(d => d.IsActive == search.IsActive.Value);

            if (search.IsAvailableForOnline.HasValue)
                query = query.Where(d => d.IsAvailableForOnline == search.IsAvailableForOnline.Value);

            if (search.Gender.HasValue)
                query = query.Where(d => d.Gender == search.Gender.Value);

            if (!string.IsNullOrWhiteSpace(search.Specialization))
            {
                var spec = search.Specialization.Trim();
                query = query.Where(d => d.Specialization != null
                                      && d.Specialization.Contains(spec));
            }

            if (search.MinFee.HasValue)
                query = query.Where(d => d.ConsultationFee >= search.MinFee.Value);

            if (search.MaxFee.HasValue)
                query = query.Where(d => d.ConsultationFee <= search.MaxFee.Value);

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.Trim();
                query = query.Where(d =>
                    d.DoctorCode.Contains(kw) ||
                    d.Slug.Contains(kw) ||
                    (d.Specialization != null && d.Specialization.Contains(kw)));
            }

            query = query
                .OrderBy(d => d.SortOrder)
                .ThenByDescending(d => d.CreatedDate);

            // ==================== BƯỚC 2: Đếm + Paging ====================
            var total = await query.CountAsync(cancellationToken);

            var doctors = await query
                .Skip((search.PageIndex - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync(cancellationToken);

            // ==================== BƯỚC 3: Batch load Accounts ====================
            var accountIds = doctors.Select(d => d.AccountId).Distinct().ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // ==================== BƯỚC 4: Batch load Departments ====================
            var departmentIds = doctors.Select(d => d.DepartmentId).Distinct().ToList();

            var departmentNames = await _departmentRepo.GetQueryable()
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

            // ==================== BƯỚC 5: Map ====================
            var items = doctors
                .Select(d => DoctorMapper.ToAdminListDto(
                    d,
                    accountFullName: accountNames.GetValueOrDefault(d.AccountId, string.Empty),
                    departmentName: departmentNames.GetValueOrDefault(d.DepartmentId)))
                .ToList();

            return new PagedResultDto<DoctorAdminListDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = search.PageIndex,
                PageSize = search.PageSize
            };
        }
    }
}
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Doctor;
using BaseClinic.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    /// <summary>
    /// Danh sách bác sĩ công khai (landing page).
    /// Chỉ trả về doctors active.
    /// 
    /// SORT: SortOrder ASC → CreatedDate DESC.
    /// </summary>
    public record GetPublicDoctorsQuery(DoctorSearchDto Search)
        : IRequest<PagedResultDto<DoctorPublicListDto>>;

    public class GetPublicDoctorsQueryHandler
        : IRequestHandler<GetPublicDoctorsQuery, PagedResultDto<DoctorPublicListDto>>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetPublicDoctorsQueryHandler(
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<PagedResultDto<DoctorPublicListDto>> Handle(
            GetPublicDoctorsQuery request,
            CancellationToken cancellationToken)
        {
            var search = request.Search;

            // ==================== BƯỚC 1: Query doctors ====================
            // Chỉ lấy active, không cho IncludeDeleted
            var query = _doctorRepo.GetQueryable()
                .Where(d => d.IsActive);

            if (search.DepartmentId.HasValue)
                query = query.Where(d => d.DepartmentId == search.DepartmentId.Value);

            if (!string.IsNullOrWhiteSpace(search.Specialization))
            {
                var spec = search.Specialization.Trim();
                query = query.Where(d => d.Specialization != null
                                      && d.Specialization.Contains(spec));
            }

            if (search.Gender.HasValue)
                query = query.Where(d => d.Gender == search.Gender.Value);

            if (search.MinFee.HasValue)
                query = query.Where(d => d.ConsultationFee >= search.MinFee.Value);

            if (search.MaxFee.HasValue)
                query = query.Where(d => d.ConsultationFee <= search.MaxFee.Value);

            // Keyword filter: DoctorCode hoặc Specialization (FullName ở Account → filter sau)
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

            var departments = await _departmentRepo.GetQueryable()
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d, cancellationToken);

            // ==================== BƯỚC 5: Map ====================
            var items = doctors
                .Select(d => DoctorMapper.ToPublicListDto(
                    d,
                    accountFullName: accountNames.GetValueOrDefault(d.AccountId, string.Empty),
                    department: departments.GetValueOrDefault(d.DepartmentId)))
                .ToList();

            return new PagedResultDto<DoctorPublicListDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = search.PageIndex,
                PageSize = search.PageSize
            };
        }
    }
}
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Department;
using BaseClinic.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Queries
{
    /// <summary>
    /// Lấy danh sách khoa có phân trang — dùng cho Admin.
    /// 
    /// SORT: Luôn theo SortOrder ASC, sau đó CreatedDate DESC.
    /// 
    /// FILTER:
    /// - Keyword (Name hoặc DepartmentCode)
    /// - IsActive
    /// - IncludeDeleted (chỉ Admin — dùng IgnoreQueryFilters)
    /// </summary>
    public record GetAllDepartmentsQuery(DepartmentSearchDto Search)
        : IRequest<PagedResultDto<DepartmentAdminListDto>>;

    public class GetAllDepartmentsQueryHandler
        : IRequestHandler<GetAllDepartmentsQuery, PagedResultDto<DepartmentAdminListDto>>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;

        public GetAllDepartmentsQueryHandler(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
        }

        public async Task<PagedResultDto<DepartmentAdminListDto>> Handle(
            GetAllDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            var search = request.Search;

            // ==================== BƯỚC 1: Query departments ====================
            var query = search.IncludeDeleted
                ? _departmentRepo.GetQueryable().IgnoreQueryFilters()
                : _departmentRepo.GetQueryable();

            // Filters
            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.Trim();
                query = query.Where(d =>
                    d.Name.Contains(kw) ||
                    d.DepartmentCode.Contains(kw));
            }

            if (search.IsActive.HasValue)
                query = query.Where(d => d.IsActive == search.IsActive.Value);

            // Sort: SortOrder ASC → CreatedDate DESC
            query = query
                .OrderBy(d => d.SortOrder)
                .ThenByDescending(d => d.CreatedDate);

            // ==================== BƯỚC 2: Đếm tổng ====================
            var total = await query.CountAsync(cancellationToken);

            // ==================== BƯỚC 3: Paging ====================
            var departments = await query
                .Skip((search.PageIndex - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync(cancellationToken);

            // ==================== BƯỚC 4: Đếm ActiveDoctorCount theo batch ====================
            var departmentIds = departments.Select(d => d.Id).ToList();

            var doctorCounts = await _doctorRepo.GetQueryable()
                .Where(dr => departmentIds.Contains(dr.DepartmentId) && dr.IsActive)
                .GroupBy(dr => dr.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DepartmentId, x => x.Count, cancellationToken);

            // ==================== BƯỚC 5: Map ====================
            var items = departments
                .Select(d => DepartmentMapper.ToAdminListDto(
                    d,
                    activeDoctorCount: doctorCounts.GetValueOrDefault(d.Id, 0)))
                .ToList();

            return new PagedResultDto<DepartmentAdminListDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = search.PageIndex,
                PageSize = search.PageSize
            };
        }
    }
}
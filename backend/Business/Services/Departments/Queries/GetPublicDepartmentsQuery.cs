using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Department;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Queries
{
    /// <summary>
    /// Lấy danh sách khoa công khai — dùng cho Landing page.
    /// Chỉ trả về khoa đang hoạt động (IsActive = true).
    /// </summary>
    public record GetPublicDepartmentsQuery(DepartmentSearchDto Search)
        : IRequest<PagedResultDto<DepartmentPublicListDto>>;

    public class GetPublicDepartmentsQueryHandler
        : IRequestHandler<GetPublicDepartmentsQuery, PagedResultDto<DepartmentPublicListDto>>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;

        public GetPublicDepartmentsQueryHandler(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
        }

        public async Task<PagedResultDto<DepartmentPublicListDto>> Handle(
            GetPublicDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            var search = request.Search;

            // Chỉ query khoa active, không cho IncludeDeleted
            var query = _departmentRepo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.Trim();
                query = query.Where(d =>
                    d.Name.Contains(kw) ||
                    d.DepartmentCode.Contains(kw));
            }

            query = query
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name);

            var total = await query.CountAsync(cancellationToken);

            var departments = await query
                .Skip((search.PageIndex - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync(cancellationToken);

            // Đếm ActiveDoctorCount
            var departmentIds = departments.Select(d => d.Id).ToList();

            var doctorCounts = await _doctorRepo.GetQueryable()
                .Where(dr => departmentIds.Contains(dr.DepartmentId) && dr.IsActive)
                .GroupBy(dr => dr.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DepartmentId, x => x.Count, cancellationToken);

            var items = departments
                .Select(d => DepartmentMapper.ToPublicListDto(
                    d,
                    activeDoctorCount: doctorCounts.GetValueOrDefault(d.Id, 0)))
                .ToList();

            return new PagedResultDto<DepartmentPublicListDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = search.PageIndex,
                PageSize = search.PageSize
            };
        }
    }
}
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Department;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Queries
{
    /// <summary>
    /// Lấy danh sách khoa đang hoạt động — dùng cho dropdown/select.
    /// 
    /// SORT: SortOrder ASC.
    /// Không paging — số khoa thường ít.
    /// </summary>
    public record GetActiveDepartmentsQuery : IRequest<List<DepartmentLookupDto>>;

    public class GetActiveDepartmentsQueryHandler
        : IRequestHandler<GetActiveDepartmentsQuery, List<DepartmentLookupDto>>
    {
        private readonly IDepartmentRepository _departmentRepo;

        public GetActiveDepartmentsQueryHandler(IDepartmentRepository departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        public async Task<List<DepartmentLookupDto>> Handle(
            GetActiveDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            var departments = await _departmentRepo.GetQueryable()
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.Name)
                .ToListAsync(cancellationToken);

            return departments
                .Select(DepartmentMapper.ToLookupDto)
                .ToList();
        }
    }
}
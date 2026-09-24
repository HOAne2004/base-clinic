using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Queries
{
    // Query dưới dạng record
    public record GetAllDepartmentsQuery() : IRequest<List<DepartmentDetailDto>>;

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDetailDto>>
    {
        private readonly ClinicDbContext _context;

        public GetAllDepartmentsQueryHandler(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDetailDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Select(d => new DepartmentDetailDto(
                    d.Id,
                    d.Name,
                    d.DepartmentCode,
                    d.ImageUrl,
                    d.Description,
                    d.IsActive
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
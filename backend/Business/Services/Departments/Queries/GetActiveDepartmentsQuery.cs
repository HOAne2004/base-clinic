using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Queries
{
    public record GetActiveDepartmentsQuery : IRequest<List<DepartmentLookupDto>>
    {
    }
    public class GetActiveDepartmentsQueryHandler : IRequestHandler<GetActiveDepartmentsQuery, List<DepartmentLookupDto>>
    {
        private readonly ClinicDbContext _context;

        public GetActiveDepartmentsQueryHandler(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentLookupDto>> Handle(GetActiveDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(d => d.IsActive)
                .Select(d => new DepartmentLookupDto
                ( 
                    d.Id,
                    d.Name
                )).ToListAsync(cancellationToken);
        }
    }
}

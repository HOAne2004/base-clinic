using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Roles.Queries
{
    public record GetAllRolesQuery() : IRequest<List<RoleDetailDto>>;

    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDetailDto>>
    {
        private readonly ClinicDbContext _context;
        public GetAllRolesQueryHandler(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleDetailDto>> Handle(GetAllRolesQuery request, CancellationToken cancellation)
        {
            return await _context.Roles
                .Select(r => new RoleDetailDto(
                    r.Id,
                    r.Name,
                    r.Description,
                    r.IsSystemRole)).ToListAsync(cancellation);
        }
    }
}

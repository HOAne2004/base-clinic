using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Roles.Queries
{
    // Yêu cầu trả về danh sách các quyền đã được gom nhóm
    public class GetAllPermissionsQuery : IRequest<List<PermissionGroupDto>>
    {
    }
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionGroupDto>>
    {
        private readonly ClinicDbContext _context;

        public GetAllPermissionsQueryHandler(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<PermissionGroupDto>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _context.Permissions.ToListAsync(cancellationToken);

            // Dùng LINQ để gom nhóm theo Resource
            var groupedPermissions = permissions
                .GroupBy(p => p.Resource)
                .Select(g => new PermissionGroupDto
                {
                    Resource = g.Key,
                    Permissions = g.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    }).ToList()
                })
                .ToList();

            return groupedPermissions;
        }
    }
}

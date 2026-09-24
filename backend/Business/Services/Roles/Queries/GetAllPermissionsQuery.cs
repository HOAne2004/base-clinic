using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using BaseClinic.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Roles.Queries
{
    // Yêu cầu trả về danh sách các quyền đã được gom nhóm
    public record GetAllPermissionsQuery : IRequest<List<PermissionGroupDto>>
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
         (
             g.Key,
             g.Select(p => new PermissionDto
             (
                 p.Id,
                 p.Name,
                 p.Description
             )).ToList() // .ToList() thứ nhất: Ép kiểu cho danh sách PermissionDto con
         )).ToList();    // .ToList() THỨ HAI Ở ĐÂY: Ép kiểu cho toàn bộ PermissionGroupDto

            return groupedPermissions;
        }
    }
}

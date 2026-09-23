using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Queries
{
    // Yêu cầu trả về danh sách ID các quyền mà Role này đang sở hữu
    public class GetRolePermissionsQuery : IRequest<List<Guid>>
    {
        public Guid RoleId { get; set; }
    }

    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, List<Guid>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolePermissionsQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Guid>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            // Sử dụng lại hàm Include RolePermissions đã viết ở Bước 3 của command trước đó
            var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId, cancellationToken)
                ?? throw new InvalidOperationException("Không tìm thấy chức danh này.");

            // Chỉ trích xuất và trả về danh sách Guid của các quyền
            return role.RolePermissions.Select(rp => rp.PermissionId).ToList();
        }
    }
}

using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Commands
{
    public record AssignRolePermissionsCommand (Guid RoleId, List<Guid> PermissionIds) : IRequest<bool>;


    public class AssignRolePermissionsCommandHandler : IRequestHandler<AssignRolePermissionsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;

        public AssignRolePermissionsCommandHandler(
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<bool> Handle(AssignRolePermissionsCommand request, CancellationToken cancellation)
        {
            await _unitOfWork.BeginTransactionAsync(cancellation);

            try
            {
                // 1. Lấy Role kèm theo danh sách các quyền đang có (Eager Loading)
                var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId, cancellation)
                    ?? throw new InvalidOperationException("Không tìm thấy chức danh (Role) này.");

                // 2. Chặn cập nhật quyền nếu đây là System Role (như Admin gốc) để bảo vệ hệ thống
                if (role.IsSystemRole)
                {
                    throw new InvalidOperationException("Không được phép thay đổi quyền của System Role.");
                }

                // 3. Lấy ra danh sách các PermissionId hiện tại đang nằm trong Role
                var currentPermissonIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();

                // 4. Tìm các quyền bị Admin BỎ TÍCH trên giao diện (Có trong DB nhưng không có trong Request)
                var permissionToRemove = currentPermissonIds.Except(request.PermissionIds).ToList();

                // 5. Tìm các quyền được Admin TÍCH MỚI (Có trong Request nhưng chưa có trong DB)
                var permissionToAdd = request.PermissionIds.Except(currentPermissonIds).ToList();

                // 6. Cập nhật Entity thông qua các method Domain
                foreach (var permissionId in permissionToRemove)
                {
                    role.RemovePermission(permissionId);
                }
                foreach (var permissionId in permissionToAdd)
                {
                    role.AssignPermission(permissionId);
                }

                await _unitOfWork.CommitTransactionAsync(cancellation);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellation);
                throw;
            }
        }
    }
}

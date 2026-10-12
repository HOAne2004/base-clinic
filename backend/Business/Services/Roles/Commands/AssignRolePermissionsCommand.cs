using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Commands
{
    /// <summary>
    /// Gán/thu hồi permission cho Role.
    /// </summary>
    public record AssignRolePermissionsCommand(
        Guid RoleId,
        List<Guid> PermissionIds,
        Guid GrantedByAccountId    // ← AccountId của Admin đang thao tác
    ) : IRequest<bool>;

    public class AssignRolePermissionsCommandHandler
        : IRequestHandler<AssignRolePermissionsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepo;

        public AssignRolePermissionsCommandHandler(
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepo)
        {
            _unitOfWork = unitOfWork;
            _roleRepo = roleRepo;
        }

        public async Task<bool> Handle(
            AssignRolePermissionsCommand request,
            CancellationToken cancellation)
        {
            await _unitOfWork.BeginTransactionAsync(cancellation);
            try
            {
                var role = await _roleRepo.GetByIdWithPermissionsAsync(
                    request.RoleId, cancellation)
                    ?? throw new NotFoundException("Không tìm thấy chức danh (Role).");

                // ⚡ Dùng SyncPermissions — đã có sẵn trong Role
                //    Sync = add mới + remove cũ, tối ưu hơn vòng lặp thủ công
                role.SyncPermissions(
                    permissionIds: request.PermissionIds,
                    grantedBy: request.GrantedByAccountId);

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
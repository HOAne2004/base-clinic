using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Commands
{
    public record DeleteRoleCommand (Guid Id) : IRequest<bool>;

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellation)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id);

            if (role == null)
            {
                throw new InvalidOperationException("Role không tồn tại.");
            }

            if (role.IsSystemRole)
            {
                throw new InvalidOperationException("Không được phép xóa System Role.");
            }

            _roleRepository.Remove(role);

            await _unitOfWork.SaveChangesAsync(cancellation);

            return true;
        }
    }
}

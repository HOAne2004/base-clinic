using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Models;
using BaseClinic.Domain.Entities;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Commands
{
    public record CreateRoleCommand(string Name, string? Description, bool IsSystemRole) : IRequest<Guid>;

    public class CreateRoleCommandHander : IRequestHandler<CreateRoleCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;

        public CreateRoleCommandHander(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellation)
        {
            var roleExist = await _roleRepository.GetByNameAsync(request.Name, cancellation);
            if (roleExist != null)
            {
                throw new InvalidOperationException("Role đã tồn tại.");
            }

            var role = new Role(
                request.Name,
                request.Description ?? string.Empty, // Đảm bảo nếu null
                request.IsSystemRole);

            _roleRepository.Add(role);
            await _unitOfWork.SaveChangesAsync(cancellation);
            return role.Id;
        }
    }
}

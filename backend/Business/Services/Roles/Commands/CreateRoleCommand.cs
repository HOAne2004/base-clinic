using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Commands
{
    public record CreateRoleCommand(
        string Code,            // mã role, VD: "CUSTOM_ROLE"
        string Name,
        string? Description,
        int DisplayOrder,       
        int Priority,          
        bool IsSystemRole
    ) : IRequest<Guid>;

    public class CreateRoleCommandHandler
        : IRequestHandler<CreateRoleCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepo;

        public CreateRoleCommandHandler(
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepo)
        {
            _unitOfWork = unitOfWork;
            _roleRepo = roleRepo;
        }

        public async Task<Guid> Handle(
            CreateRoleCommand request,
            CancellationToken cancellation)
        {
            // Check trùng Code
            var existByCode = await _roleRepo.GetByCodeAsync(
                request.Code, cancellation);

            if (existByCode != null)
                throw new DomainException($"Mã role '{request.Code}' đã tồn tại.");

            // Check trùng Name
            var existByName = await _roleRepo.GetByNameAsync(
                request.Name, cancellation);

            if (existByName != null)
                throw new DomainException($"Tên role '{request.Name}' đã tồn tại.");

            var role = new Role(
                code: request.Code,
                name: request.Name,
                description: request.Description,
                displayOrder: request.DisplayOrder,
                priority: request.Priority,
                isSystemRole: request.IsSystemRole);

            _roleRepo.Add(role);
            await _unitOfWork.SaveChangesAsync(cancellation);

            return role.Id;
        }
    }
}
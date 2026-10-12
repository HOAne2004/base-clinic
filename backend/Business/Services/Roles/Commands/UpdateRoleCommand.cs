using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Commands
{
    public record UpdateRoleCommand(
        Guid Id,
        string Name,
        string? Description
    ) : IRequest<bool>;

    public class UpdateRoleCommandHandler
        : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepo;

        public UpdateRoleCommandHandler(
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepo)
        {
            _unitOfWork = unitOfWork;
            _roleRepo = roleRepo;
        }

        public async Task<bool> Handle(
            UpdateRoleCommand request,
            CancellationToken cancellation)
        {
            var role = await _roleRepo.GetByIdAsync(request.Id)
                ?? throw new NotFoundException("Không tìm thấy Role tương ứng.");

            //    SetName/SetDescription đã có check IsSystemRole bên trong
            role.SetName(request.Name);
            role.SetDescription(request.Description);

            await _unitOfWork.SaveChangesAsync(cancellation);
            return true;
        }
    }
}
using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Roles.Commands
{
    public record UpdateRoleCommand (Guid Id, string Name, string? Description) : IRequest<bool>;

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellation)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id);
            if (role == null)
            {
                throw new InvalidOperationException("Không tìm thấy Role tương ứng");
            }

            role.Update(request.Name, request.Description);
            await _unitOfWork.SaveChangesAsync(cancellation);

            return true;

        }
    }
}

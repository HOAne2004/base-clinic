using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record DeactivateDepartmentCommand (Guid Id) : IRequest<bool>;

    public class DeactivateDepartmentCommandHandler : IRequestHandler<DeactivateDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeactivateDepartmentCommand request, CancellationToken cancellation)
        {
            var department = await _departmentRepository.GetByIdAsync(request.Id, cancellation);
            if (department == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin khoa.");
            }

            department.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellation);
            return true;
        }
    }
}

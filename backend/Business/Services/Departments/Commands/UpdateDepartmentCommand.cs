using BaseClinic.Business.Interfaces;
using BaseClinic.DataAccess.Repositories;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record UpdateDepartmentCommand (Guid Id, string Name, string? ImageUrl, string? Description) : IRequest<bool>;
    
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _repository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellation)
        {
            var department = await _repository.GetByIdAsync(request.Id, cancellation)
                ?? throw new InvalidOperationException("Không tìm thấy thông tin khoa.");

            department.UpdateName(request.Name);
            department.UpdateDetails(request.ImageUrl, request.Description);

            await _unitOfWork.SaveChangesAsync(cancellation);
            return true;
        }

    }
}

using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Entities;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record CreateDeparementCommand 
        (string Name,
        string DepartmentCode,
        string? ImageUrl,
        string? Description) : IRequest<Guid>;
    

    public class CreateDeparementCommandHandler : IRequestHandler<CreateDeparementCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;

        public CreateDeparementCommandHandler(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository)
        {
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
        }

        public async Task<Guid> Handle(CreateDeparementCommand request, CancellationToken cancellation)
        {
            bool isExist = await _departmentRepository.IsCodeExistAsync(request.DepartmentCode, cancellation);
            if (!isExist)
            {
                throw new InvalidOperationException($"Mã khoa '{request.DepartmentCode}' đã tồn tại trong hệ thống.");
            }

            var department = new Department(
                name: request.Name,
                departmentCode: request.DepartmentCode,
                imageUrl: request.ImageUrl,
                description: request.Description
                );

            _departmentRepository.Add(department);
            await _unitOfWork.SaveChangesAsync(cancellation);

            return department.Id;
        }
    }
}

using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record CreateDepartmentCommand
        (string Name,
        string DepartmentCode,
        string? ImageUrl,
        string? ShortDescription,
        string? FullDescription,
        string? Location,
        string? PhoneNumber) : IRequest<Guid>;
    

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;

        public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository)
        {
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
        }

        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellation)
        {
            bool isExist = await _departmentRepository.IsCodeExistAsync(request.DepartmentCode, cancellation);
            if (isExist)
            {
                throw new InvalidOperationException($"Mã khoa '{request.DepartmentCode}' đã tồn tại trong hệ thống.");
            }

            var department = new Department(
                name: request.Name,
                departmentCode: request.DepartmentCode,
                imageUrl: request.ImageUrl,
                shortDescription: request.ShortDescription,
                fullDescription: request.FullDescription,
                location: request.Location,
                phoneNumber: request.PhoneNumber
                );

            _departmentRepository.Add(department);
            await _unitOfWork.SaveChangesAsync(cancellation);

            return department.Id;
        }
    }
}

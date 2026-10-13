using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record CreateDepartmentCommand(
        string Name,
        string DepartmentCode,
        string? ImageUrl,
        string? ShortDescription,
        string? FullDescription,
        string? Location,
        string? PhoneNumber,
        bool Is24Hours = false,
        TimeOnly? OpeningTime = null,
        TimeOnly? ClosingTime = null,
        int SortOrder = Department.DefaultSortOrder
    ) : IRequest<Guid>;

    public class CreateDepartmentCommandHandler
        : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepo;

        public CreateDepartmentCommandHandler(
            IUnitOfWork unitOfWork,
            IDepartmentRepository departmentRepo)
        {
            _unitOfWork = unitOfWork;
            _departmentRepo = departmentRepo;
        }

        public async Task<Guid> Handle(
            CreateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Check trùng Code
            var isExist = await _departmentRepo.IsCodeExistAsync(
                request.DepartmentCode, cancellationToken);

            if (isExist)
                throw new DomainException(
                    $"Mã khoa '{request.DepartmentCode}' đã tồn tại trong hệ thống.");

            // 2. Tạo Department
            var department = new Department(
                name: request.Name,
                departmentCode: request.DepartmentCode,
                imageUrl: request.ImageUrl,
                shortDescription: request.ShortDescription,
                fullDescription: request.FullDescription,
                location: request.Location,
                phoneNumber: request.PhoneNumber,
                is24Hours: request.Is24Hours,
                openingTime: request.OpeningTime,
                closingTime: request.ClosingTime,
                sortOrder: request.SortOrder);

            _departmentRepo.Add(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }
}
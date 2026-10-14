using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record CreateDepartmentCommand(
        string Name,
        string DepartmentCode,
        string? Slug,
        string? ImageUrl,
        string? ShortDescription,
        string? FullDescription,
        string? Location,
        string? PhoneNumber,
        bool Is24Hours = false,
        TimeOnly? OpeningTime = null,
        TimeOnly? ClosingTime = null,
        int SortOrder = Department.DefaultSortOrder,
        bool OnlineBookingEnabled = true
    ) : IRequest<Guid>;

    public class CreateDepartmentCommandHandler
        : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly ISlugGenerator _slugGenerator;

        public CreateDepartmentCommandHandler(
            IUnitOfWork unitOfWork,
            IDepartmentRepository departmentRepo,
            ISlugGenerator slugGenerator)
        {
            _unitOfWork = unitOfWork;
            _departmentRepo = departmentRepo;
            _slugGenerator = slugGenerator;
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

            var slugSource = !string.IsNullOrWhiteSpace(request.Slug)
               ? request.Slug
               : request.Name;

            var uniqueSlug = await _slugGenerator.GenerateUniqueDepartmentSlugAsync(
                input: slugSource,
                excludeId: null,
                cancellationToken: cancellationToken);
            // 2. Tạo Department
            var department = new Department(
                name: request.Name,
                departmentCode: request.DepartmentCode,
                slug: uniqueSlug,
                imageUrl: request.ImageUrl,
                shortDescription: request.ShortDescription,
                fullDescription: request.FullDescription,
                location: request.Location,
                phoneNumber: request.PhoneNumber,
                is24Hours: request.Is24Hours,
                openingTime: request.OpeningTime,
                closingTime: request.ClosingTime,
                sortOrder: request.SortOrder,
                onlineBookingEnabled: request.OnlineBookingEnabled);

            _departmentRepo.Add(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }
}
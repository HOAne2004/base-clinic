using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record UpdateDepartmentCommand(
        Guid Id,
        string Name,
        string? Slug,
        string? ImageUrl,
        string? ShortDescription,
        string? FullDescription,
        string? Location,
        string? PhoneNumber,
        bool Is24Hours,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        int SortOrder
    ) : IRequest<bool>;

    public class UpdateDepartmentCommandHandler
        : IRequestHandler<UpdateDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISlugGenerator _slugGenerator;

        public UpdateDepartmentCommandHandler(
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork,
            ISlugGenerator slugGenerator)
        {
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
            _slugGenerator = slugGenerator;
        }

        public async Task<bool> Handle(
            UpdateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _departmentRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy thông tin khoa.");

            // Xác định có cần regenerate slug không
            bool nameChanged = !string.Equals(
                request.Name?.Trim(),
                department.Name,
                StringComparison.Ordinal);

            bool adminOverrideSlug = !string.IsNullOrWhiteSpace(request.Slug);

            department.UpdateName(request.Name);
            department.UpdateDetails(
                request.ImageUrl,
                request.ShortDescription,
                request.FullDescription,
                request.Location,
                request.PhoneNumber);
            department.UpdateOperatingHours(
                request.Is24Hours,
                request.OpeningTime,
                request.ClosingTime);
            department.SetSortOrder(request.SortOrder);

            if (adminOverrideSlug || nameChanged)
            {
                var slugSource = adminOverrideSlug
                    ? request.Slug!
                    : request.Name;

                var newSlug = await _slugGenerator.GenerateUniqueDepartmentSlugAsync(
                    input: slugSource,
                    excludeId: department.Id,
                    cancellationToken: cancellationToken);

                department.UpdateSlug(newSlug);
            }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
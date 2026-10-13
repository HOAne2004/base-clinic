using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    public record UpdateDepartmentCommand(
        Guid Id,
        string Name,
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

        public UpdateDepartmentCommandHandler(
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _departmentRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy thông tin khoa.");

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

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
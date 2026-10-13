using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Departments.Commands
{
    /// <summary>
    /// Kích hoạt lại khoa đang tạm ngưng.
    /// 
    /// KHÔNG CASCADE: Doctors phải được Admin active thủ công.
    /// Lý do: Có thể có Doctor đã nghỉ việc trước khi khoa deactivate.
    /// </summary>
    public record ActivateDepartmentCommand(Guid Id) : IRequest<bool>;

    public class ActivateDepartmentCommandHandler
        : IRequestHandler<ActivateDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateDepartmentCommandHandler(
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ActivateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _departmentRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy thông tin khoa.");

            if (department.IsActive)
                throw new DomainException("Khoa này đã đang hoạt động.");

            department.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
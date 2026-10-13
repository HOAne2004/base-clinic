using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Commands
{
    /// <summary>
    /// Khôi phục khoa đã bị xóa mềm.
    /// 
    /// LƯU Ý: Phải dùng IgnoreQueryFilters để tìm entity đã xóa.
    /// </summary>
    public record RestoreDepartmentCommand(Guid Id) : IRequest<bool>;

    public class RestoreDepartmentCommandHandler
        : IRequestHandler<RestoreDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RestoreDepartmentCommandHandler(
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            RestoreDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            // Phải bypass global query filter để tìm department đã xóa mềm
            var department = await _departmentRepo.GetQueryableWithTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy thông tin khoa.");

            if (!department.IsDeleted)
                throw new DomainException("Khoa này chưa bị xóa, không cần khôi phục.");

            _departmentRepo.Restore(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
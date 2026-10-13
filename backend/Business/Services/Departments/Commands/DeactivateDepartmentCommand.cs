using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Commands
{
    /// <summary>
    /// Vô hiệu hóa khoa (tạm ngưng).
    /// 
    /// CASCADE: Tất cả Doctor trong khoa bị set IsActive = false.
    /// (Bác sĩ vẫn tồn tại, chỉ không còn active).
    /// </summary>
    public record DeactivateDepartmentCommand(Guid Id) : IRequest<bool>;

    public class DeactivateDepartmentCommandHandler
        : IRequestHandler<DeactivateDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateDepartmentCommandHandler(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeactivateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _departmentRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy thông tin khoa.");

            if (!department.IsActive)
                throw new DomainException("Khoa này đã ở trạng thái không hoạt động.");

            // 1. Deactivate department
            department.Deactivate();

            // 2. Cascade: deactivate tất cả doctors đang active trong khoa
            var activeDoctors = await _doctorRepo.GetQueryableWithTracking()
                .Where(d => d.DepartmentId == department.Id && d.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var doctor in activeDoctors)
                doctor.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
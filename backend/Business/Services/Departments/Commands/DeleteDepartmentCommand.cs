using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Commands
{
    /// <summary>
    /// Xóa mềm khoa.
    /// 
    /// ĐIỀU KIỆN:
    /// - Không cho xóa nếu còn Doctor trong khoa (kể cả inactive).
    /// - Phải deactivate + chuyển hết Doctor đi trước.
    /// </summary>
    public record DeleteDepartmentCommand(Guid Id) : IRequest<bool>;

    public class DeleteDepartmentCommandHandler
        : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentCommandHandler(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await _departmentRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy thông tin khoa.");

            // Check còn Doctor nào không (bao gồm cả inactive, chưa xóa mềm)
            var hasDoctors = await _doctorRepo.GetQueryable()
                .AnyAsync(d => d.DepartmentId == department.Id, cancellationToken);

            if (hasDoctors)
                throw new DomainException(
                    "Không thể xóa khoa vì vẫn còn bác sĩ trong khoa. " +
                    "Vui lòng chuyển bác sĩ sang khoa khác trước.");

            _departmentRepo.Delete(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
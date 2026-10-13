using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Admin chuyển bác sĩ sang khoa khác.
    /// </summary>
    public record TransferDoctorDepartmentCommand(
        Guid DoctorId,
        Guid NewDepartmentId
    ) : IRequest<bool>;

    public class TransferDoctorDepartmentCommandHandler
        : IRequestHandler<TransferDoctorDepartmentCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public TransferDoctorDepartmentCommandHandler(
            IDoctorRepository doctorRepo,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            TransferDoctorDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByIdAsync(request.DoctorId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            if (doctor.DepartmentId == request.NewDepartmentId)
                throw new DomainException("Bác sĩ đã ở khoa này.");

            var newDepartment = await _departmentRepo.GetByIdAsync(
                request.NewDepartmentId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy khoa mới.");

            if (!newDepartment.IsActive)
                throw new DomainException(
                    "Không thể chuyển bác sĩ sang khoa đang tạm ngưng.");

            doctor.TransferToDepartment(request.NewDepartmentId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
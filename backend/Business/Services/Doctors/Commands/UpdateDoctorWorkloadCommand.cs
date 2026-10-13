using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Cập nhật WORKLOAD: MaxPatientsPerDay, JoinDate, IsAvailableForOnline.
    /// </summary>
    public record UpdateDoctorWorkloadCommand(
        Guid Id,
        int? MaxPatientsPerDay,
        DateTime? JoinDate,
        bool IsAvailableForOnline
    ) : IRequest<bool>;

    public class UpdateDoctorWorkloadCommandHandler
        : IRequestHandler<UpdateDoctorWorkloadCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorWorkloadCommandHandler(
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateDoctorWorkloadCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            doctor.UpdateWorkload(request.MaxPatientsPerDay, request.JoinDate);
            doctor.SetOnlineAvailability(request.IsAvailableForOnline);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
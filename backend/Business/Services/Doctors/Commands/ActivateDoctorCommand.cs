using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    public record ActivateDoctorCommand(Guid Id) : IRequest<bool>;

    public class ActivateDoctorCommandHandler
        : IRequestHandler<ActivateDoctorCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateDoctorCommandHandler(
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ActivateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            if (doctor.IsActive)
                throw new DomainException("Bác sĩ này đã đang hoạt động.");

            doctor.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
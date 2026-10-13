using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Cập nhật THÔNG TIN NGHỀ NGHIỆP: Title, Specialization, Years, Biography, Languages.
    /// </summary>
    public record UpdateDoctorProfessionalInfoCommand(
        Guid Id,
        string? Title,
        string? Specialization,
        int? YearsOfExperience,
        string? Biography,
        string? Languages
    ) : IRequest<bool>;

    public class UpdateDoctorProfessionalInfoCommandHandler
        : IRequestHandler<UpdateDoctorProfessionalInfoCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorProfessionalInfoCommandHandler(
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateDoctorProfessionalInfoCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            doctor.UpdateProfessionalInfo(
                title: request.Title,
                specialization: request.Specialization,
                yearsOfExperience: request.YearsOfExperience,
                biography: request.Biography,
                languages: request.Languages);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
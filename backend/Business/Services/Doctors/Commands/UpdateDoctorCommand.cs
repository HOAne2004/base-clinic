using BaseClinic.Business.Interfaces;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    // Làm phẳng các thuộc tính, không bọc qua DoctorDto
    public record UpdateDoctorCommand(
        Guid Id,
        string? Avatar,
        string? FullAddress,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? LicenseNumber,
        decimal? ConsultationFee,
        string? Description
    ) : IRequest<bool>;

    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateDoctorCommand request, CancellationToken cancellation)
        {
            var doctor = await _doctorRepository.GetByIdAsync(request.Id, cancellation);
            if (doctor == null)
            {
                throw new InvalidOperationException("Bác sĩ không tồn tại.");
            }

            doctor.UpdateProfile(
                request.Avatar,
                request.FullAddress,
                request.Gender,
                request.DateOfBirth,
                request.IdentityNumber,
                request.Description);

            doctor.UpdateLicenseNumber(request.LicenseNumber);

            // Ép kiểu an toàn (tránh lỗi biên dịch)
            doctor.UpdateConsultationFee(request.ConsultationFee ?? 0);

            await _unitOfWork.SaveChangesAsync(cancellation);

            return true;
        }
    }
}
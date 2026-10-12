using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    public record UpdateDoctorCommand(
        Guid Id,
        string? Avatar,
        string? FullAddress,
        bool? Gender,
        DateTime? DateOfBirth,
        string? IdentityNumber,
        string? LicenseNumber,
        decimal? ConsultationFee,
        string? Description,
        Guid? DepartmentId
    ) : IRequest<bool>;

    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorCommandHandler(
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateDoctorCommand request,
            CancellationToken cancellation)
        {
            var doctor = await _doctorRepository.GetByIdAsync(request.Id, cancellation);
            if (doctor == null)
            {
                throw new NotFoundException("Bác sĩ không tồn tại.");
            }

            // ==================== CẬP NHẬT PROFILE ====================
            // Signature UpdateProfile của Doctor: (avatar, fullAddress, gender, dob, identityNumber, description)
            doctor.UpdateProfile(
                request.Avatar,
                request.FullAddress,
                request.Gender,
                request.DateOfBirth,
                request.IdentityNumber,
                request.Description);

            // ==================== CẬP NHẬT LICENSE ====================
            // Giữ nguyên issue/expiry/issuedBy cũ — chỉ update số license.
            if (!string.IsNullOrWhiteSpace(request.LicenseNumber))
            {
                doctor.UpdateLicense(
                    licenseNumber: request.LicenseNumber,
                    issueDate: doctor.LicenseIssueDate,
                    expiryDate: doctor.LicenseExpiryDate,
                    issuedBy: doctor.LicenseIssuedBy);
            }

            // ==================== CẬP NHẬT FEE ====================
            // Code cũ: `request.ConsultationFee ?? 0` → ghi đè fee = 0 khi null (BUG).
            if (request.ConsultationFee.HasValue)
            {
                doctor.UpdateConsultationFee(request.ConsultationFee.Value);
            }

            // ==================== CHUYỂN KHOA ====================
            if (request.DepartmentId.HasValue && request.DepartmentId.Value != Guid.Empty)
            {
                doctor.TransferToDepartment(request.DepartmentId.Value);
            }

            await _unitOfWork.SaveChangesAsync(cancellation);

            return true;
        }
    }
}
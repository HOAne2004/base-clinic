using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Commands
{
    /// <summary>
    /// Cập nhật LICENSE: LicenseNumber, IssueDate, ExpiryDate, IssuedBy.
    /// Admin update khi bác sĩ gia hạn.
    /// </summary>
    public record UpdateDoctorLicenseCommand(
        Guid Id,
        string? LicenseNumber,
        DateTime? LicenseIssueDate,
        DateTime? LicenseExpiryDate,
        string? LicenseIssuedBy
    ) : IRequest<bool>;

    public class UpdateDoctorLicenseCommandHandler
        : IRequestHandler<UpdateDoctorLicenseCommand, bool>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorLicenseCommandHandler(
            IDoctorRepository doctorRepo,
            IUnitOfWork unitOfWork)
        {
            _doctorRepo = doctorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateDoctorLicenseCommand request,
            CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy bác sĩ.");

            doctor.UpdateLicense(
                licenseNumber: request.LicenseNumber,
                issueDate: request.LicenseIssueDate,
                expiryDate: request.LicenseExpiryDate,
                issuedBy: request.LicenseIssuedBy);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
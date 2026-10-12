using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Common;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Domain.ValueObjects;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Patient tự cập nhật hồ sơ.
    /// CHỈ cho phép: AvatarUrl, Address
    /// KHÔNG cho sửa:  FullName, Gender, DateOfBirth, IdentityNumber, HealthInsuranceNumber.
    /// </summary>
    public record UpdateMyProfileCommand(
        Guid AccountId,
        string? AvatarUrl,
        AddressDto? Address
    ) : IRequest<Unit>;

    public class UpdateMyProfileCommandHandler
        : IRequestHandler<UpdateMyProfileCommand, Unit>
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMyProfileCommandHandler(
            IPatientRepository patientRepo,
            IUnitOfWork unitOfWork)
        {
            _patientRepo = patientRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            UpdateMyProfileCommand request,
            CancellationToken cancellationToken)
        {
            var patient = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken);

            if (patient == null)
                throw new DomainException(
                    "Tài khoản này chưa có hồ sơ bệnh nhân.");

            // Gọi UpdateProfile với TẤT CẢ field cũ — chỉ thay Address + AvatarUrl
            patient.UpdateProfile(
                fullName: patient.FullName,
                avatarUrl: request.AvatarUrl,
                address: MapAddress(request.Address),
                identityNumber: patient.IdentityNumber,          // Giữ nguyên
                gender: patient.Gender,
                dateOfBirth: patient.DateOfBirth,
                healthInsuranceNumber: patient.HealthInsuranceNumber);  // Giữ nguyên

            _patientRepo.Update(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private static Address? MapAddress(AddressDto? dto)
        {
            if (dto == null) return null;
            return new Address(dto.Tinh, dto.Xa, dto.Duong, dto.SoNha);
        }
    }
}
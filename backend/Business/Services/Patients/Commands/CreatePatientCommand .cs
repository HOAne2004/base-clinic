using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Domain.ValueObjects;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Tạo bệnh nhân mới.
    /// Xem comment trong CreatePatientDto để hiểu 3 trường hợp.
    /// </summary>
    public record CreatePatientCommand(CreatePatientDto Dto)
        : IRequest<CreatePatientResultDto>;

    public class CreatePatientCommandHandler
        : IRequestHandler<CreatePatientCommand, CreatePatientResultDto>
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPatientCodeGenerator _codeGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePatientCommandHandler(
            IPatientRepository patientRepo,
            IAccountRepository accountRepo,
            IRoleRepository roleRepo,
            IPatientCodeGenerator codeGenerator,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _patientRepo = patientRepo;
            _accountRepo = accountRepo;
            _roleRepo = roleRepo;
            _codeGenerator = codeGenerator;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreatePatientResultDto> Handle(
            CreatePatientCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            bool isDependent = dto.PrimaryPatientId.HasValue;

            // ==================== VALIDATE ====================
            if (isDependent && !string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new DomainException(
                    "Bệnh nhân phụ không được tạo tài khoản. " +
                    "Vui lòng để trống SĐT.");

            if (isDependent)
            {
                var primary = await _patientRepo.GetByIdAsync(
                    dto.PrimaryPatientId, cancellationToken);

                if (primary == null)
                    throw new DomainException("Không tìm thấy hồ sơ bệnh nhân chính.");
            }

            // ==================== XỬ LÝ ACCOUNT (nếu có SĐT) ====================
            Guid? accountId = null;
            string? defaultPassword = null;

            if (!isDependent && !string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                // Check SĐT chưa tồn tại
                var exists = await _accountRepo.ExistsByPhoneAsync(
                    dto.PhoneNumber, cancellationToken);

                if (exists)
                    throw new DomainException(
                        "Số điện thoại này đã được đăng ký trong hệ thống.");

                // Lấy Role PATIENT
                var patientRole = await _roleRepo.GetByCodeAsync(
                    SystemRoles.Patient, cancellationToken);

                if (patientRole == null)
                    throw new DomainException(
                        $"Role '{SystemRoles.Patient}' chưa được khởi tạo.");

                // Tạo Account — password = SĐT
                defaultPassword = dto.PhoneNumber;
                var hashedPassword = _passwordHasher.Hash(defaultPassword);

                var account = new Account(
                    fullName: dto.FullName,
                    phoneNumber: dto.PhoneNumber,
                    passwordHash: hashedPassword,
                    email: null);

                account.AssignRole(patientRole.Id);

                // Walk-in: chưa verify SĐT — bệnh nhân tự verify khi login
                // (giữ Status = Inactive)

                _accountRepo.Add(account);
                accountId = account.Id;
            }

            // ==================== TẠO PATIENT ====================
            var code = await _codeGenerator.GenerateAsync(cancellationToken);

            var patient = new Patient(
                patientCode: code,
                fullName: dto.FullName,
                accountId: accountId,
                primaryPatientId: dto.PrimaryPatientId,
                isPrimary: !isDependent);

            patient.UpdateProfile(
                fullName: dto.FullName,
                avatarUrl: null,
                address: MapAddress(dto.Address),
                identityNumber: dto.IdentityNumber,
                gender: dto.Gender,
                dateOfBirth: dto.DateOfBirth,
                healthInsuranceNumber: dto.HealthInsuranceNumber);

            _patientRepo.Add(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreatePatientResultDto(
                PatientId: patient.Id,
                PatientCode: patient.PatientCode,
                AccountId: accountId,
                DefaultPassword: defaultPassword,
                RequiresPhoneVerification: accountId != null);
        }

        private static Address? MapAddress(AddressDto? dto)
        {
            if (dto == null) return null;
            return new Address(dto.Tinh, dto.Xa, dto.Duong, dto.SoNha);
        }
    }
}
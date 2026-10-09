using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Auth;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Auth.Commands
{
    /// <summary>
    /// Command đăng ký tài khoản mới cho Patient.
    /// 
    /// LUỒNG:
    /// 1. Check SĐT chưa tồn tại.
    /// 2. Tạo Account (Inactive — chờ verify OTP).
    /// 3. Gán Role PATIENT.
    /// 4. Tạo Patient primary (chính chủ).
    /// 5. Sinh OTP verify phone.
    /// 6. Gửi SMS (fake).
    /// 7. Trả về thông tin để FE chuyển sang verify OTP.
    /// </summary>
    public record RegisterCommand(
        string FullName,
        string PhoneNumber,
        string Password
    ) : IRequest<RegisterResultDto>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResultDto>
    {
        /// <summary>Mã role mặc định cho user tự đăng ký.</summary>
        private const string DefaultRoleCode = "PATIENT";

        /// <summary>OTP hết hạn sau 5 phút.</summary>
        private const int OtpExpiryMinutes = 5;

        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IOtpCodeRepository _otpCodeRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPatientCodeGenerator _patientCodeGenerator;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IPatientRepository patientRepository,
            IOtpCodeRepository otpCodeRepository,
            IPasswordHasher passwordHasher,
            IPatientCodeGenerator patientCodeGenerator,
            ISmsService smsService,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _patientRepository = patientRepository;
            _otpCodeRepository = otpCodeRepository;
            _passwordHasher = passwordHasher;
            _patientCodeGenerator = patientCodeGenerator;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterResultDto> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Check SĐT chưa tồn tại ====================
            var existingAccount = await _accountRepository
                .GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

            if (existingAccount != null)
                throw new DomainException("Số điện thoại này đã được đăng ký.");

            // ==================== BƯỚC 2: Lấy Role PATIENT ====================
            var patientRole = await _roleRepository
                .GetByCodeAsync(DefaultRoleCode, cancellationToken);

            if (patientRole == null)
                throw new DomainException(
                    $"Role '{DefaultRoleCode}' chưa được khởi tạo trong hệ thống. " +
                    $"Vui lòng liên hệ quản trị viên.");

            // ==================== BƯỚC 3: Hash password ====================
            var hashedPassword = _passwordHasher.Hash(request.Password);

            // ==================== BƯỚC 4: Tạo Account ====================
            var account = new Account(
                fullName: request.FullName,
                phoneNumber: request.PhoneNumber,
                passwordHash: hashedPassword,
                email: null);
            // Status = Inactive tự động (constructor)

            _accountRepository.Add(account);

            // ==================== BƯỚC 5: Gán Role PATIENT ====================
            account.AssignRole(patientRole.Id);

            // ==================== BƯỚC 6: Tạo Patient primary ====================
            var patientCode = await _patientCodeGenerator.GenerateAsync(cancellationToken);

            var patient = new Patient(
                patientCode: patientCode,
                fullName: request.FullName,
                accountId: account.Id,
                primaryPatientId: null,
                isPrimary: true);

            _patientRepository.Add(patient);

            // ==================== BƯỚC 7: Vô hiệu hóa OTP cũ (nếu có) ====================
            // Không cần thiết lần đầu nhưng để chắc chắn — tránh user bấm đăng ký 2 lần.
            await _otpCodeRepository.InvalidateAllPendingAsync(
                account.Id, OtpPurpose.VerifyPhone, cancellationToken);

            // ==================== BƯỚC 8: Sinh OTP ====================
            var otp = OtpCode.Create(
                accountId: account.Id,
                purpose: OtpPurpose.VerifyPhone,
                channel: OtpChannel.Sms,
                expiryMinutes: OtpExpiryMinutes);

            _otpCodeRepository.Add(otp);

            // ==================== BƯỚC 9: Lưu DB ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ==================== BƯỚC 10: Gửi SMS (sau khi lưu) ====================
            await _smsService.SendOtpAsync(
                phoneNumber: account.PhoneNumber,
                otpCode: otp.Code,
                purpose: "xác thực số điện thoại",
                cancellationToken: cancellationToken);

            // ==================== BƯỚC 11: Trả kết quả ====================
            return new RegisterResultDto(
                AccountId: account.Id,
                PhoneNumber: account.PhoneNumber,
                OtpExpiresAt: otp.ExpiresAt,
                RequiresOtpVerification: true);
        }
    }
}
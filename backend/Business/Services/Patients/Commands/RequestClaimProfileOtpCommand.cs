using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BaseClinic.Business.Services.Patients.Commands
{
    public record RequestOtpResponse(
        string MaskedPhoneNumber,
        string MockOtp   // Production sẽ bỏ field này
    );

    public record RequestClaimProfileOtpCommand(
        Guid AccountId,
        string PatientCode
    ) : IRequest<RequestOtpResponse>;

    public class RequestClaimProfileOtpCommandHandler
        : IRequestHandler<RequestClaimProfileOtpCommand, RequestOtpResponse>
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IPatientDelegationRepository _delegationRepo;
        private readonly IMemoryCache _memoryCache;

        public RequestClaimProfileOtpCommandHandler(
            IPatientRepository patientRepo,
            IPatientDelegationRepository delegationRepo,
            IAccountRepository accountRepo,
            IMemoryCache memoryCache)
        {
            _patientRepo = patientRepo;
            _delegationRepo = delegationRepo;
            _accountRepo = accountRepo;
            _memoryCache = memoryCache;
        }

        public async Task<RequestOtpResponse> Handle(
            RequestClaimProfileOtpCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm Patient theo Code ====================
            var targetPatient = await _patientRepo.GetByCodeAsync(
                request.PatientCode, cancellationToken)
                ?? throw new DomainException("Mã bệnh nhân không tồn tại.");

            if (targetPatient.AccountId != null)
                throw new DomainException(
                    "Hồ sơ này đã được liên kết với một tài khoản khác.");

            // ==================== BƯỚC 2: Tìm người quản lý hồ sơ ====================
            var creatorAccountId = await _delegationRepo
                .GetCreatorAccountIdByTargetPatientIdAsync(
                    targetPatient.Id, cancellationToken)
                ?? throw new DomainException(
                    "Hồ sơ không có người quản lý hợp lệ. Vui lòng liên hệ lễ tân.");

            var accountA = await _accountRepo.GetByIdAsync(
                creatorAccountId, cancellationToken)
                ?? throw new DomainException(
                    "Không tìm thấy thông tin liên lạc của người quản lý hồ sơ.");

            // ==================== BƯỚC 3: Sinh OTP ====================
            string otp = new Random().Next(100000, 999999).ToString();

            // ==================== BƯỚC 4: Lưu cache 5 phút ====================
            string cacheKey = $"ClaimProfile_OTP_{request.PatientCode}";
            _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

            // ==================== BƯỚC 5: Che SĐT ====================
            string phone = accountA.PhoneNumber;
            string maskedPhone = phone.Length >= 7
                ? $"{phone[..3]}****{phone[^3..]}"
                : "****";

            // TODO: Tích hợp SMS service thật
            // await _smsService.SendAsync(accountA.PhoneNumber,
            //     $"Mã xác thực liên kết hồ sơ: {otp}");

            return new RequestOtpResponse(maskedPhone, otp);
        }
    }
}
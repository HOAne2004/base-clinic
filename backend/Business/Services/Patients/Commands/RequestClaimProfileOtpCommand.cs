using BaseClinic.Business.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BaseClinic.Business.Services.Patients.Commands
{
    // DTO trả về cho Frontend
    public record RequestOtpResponse(
        string MaskedPhoneNumber, // Hiển thị: "098****123"
        string MockOtp            // TRẢ VỀ ĐỂ TEST (Production sẽ bỏ trường này đi)
    );

    public record RequestClaimProfileOtpCommand(
        Guid AccountId,    // ID của tài khoản B (Người đang thao tác)
        string PatientCode // Mã bệnh nhân B nhập vào
    ) : IRequest<RequestOtpResponse>;

    public class RequestClaimProfileOtpCommandHandler : IRequestHandler<RequestClaimProfileOtpCommand, RequestOtpResponse>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPatientDelegationRepository _patientDelegationRepository;
        private readonly IMemoryCache _memoryCache;

        public RequestClaimProfileOtpCommandHandler(
            IPatientRepository patientRepository,
            IPatientDelegationRepository patientDelegationRepository,
            IAccountRepository accountRepository,
            IMemoryCache memoryCache)
        {
            _patientRepository = patientRepository;
            _patientDelegationRepository = patientDelegationRepository;
            _accountRepository = accountRepository;
            _memoryCache = memoryCache;
        }

        public async Task<RequestOtpResponse> Handle(RequestClaimProfileOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm hồ sơ bệnh nhân theo PatientCode
            var targetPatient = await _patientRepository.GetByPatientCodeAsync(request.PatientCode, cancellationToken)
                ?? throw new InvalidOperationException("Mã bệnh nhân không tồn tại.");

            if (targetPatient.AccountId != null)
            {
                throw new InvalidOperationException("Hồ sơ này đã được liên kết với một tài khoản khác.");
            }

            // 2. Tìm người đang quản lý hồ sơ này (Tài khoản A)
            var creatorAccountId = await _patientDelegationRepository.GetCreatorAccountIdByTargetPatientIdAsync(targetPatient.Id, cancellationToken)
                ?? throw new InvalidOperationException("Hồ sơ không có người quản lý hợp lệ. Vui lòng liên hệ lễ tân.");

            var accountA = await _accountRepository.GetByIdAsync(creatorAccountId, cancellationToken)
                ?? throw new InvalidOperationException("Không tìm thấy thông tin liên lạc của người quản lý hồ sơ.");

            // 3. Sinh mã OTP ngẫu nhiên 6 số
            string otp = new Random().Next(100000, 999999).ToString();

            // 4. Lưu vào MemoryCache với thời hạn 5 phút
            // Key format: ClaimProfile_OTP_{PatientCode}
            string cacheKey = $"ClaimProfile_OTP_{request.PatientCode}";
            _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

            // 5.Che giấu số điện thoại để bảo mật(VD: 0981234567-> 098 * ***567)
            string phone = accountA.PhoneNumber;
            string maskedPhone = phone.Length >= 7
                ? $"{phone[..3]}****{phone[^3..]}"
                : "****";

            // (Giả lập gửi SMS ở đây)
            // _smsService.SendAsync(accountA.PhoneNumber, $"Mã xác thực liên kết hồ sơ của bạn là: {otp}");

            return new RequestOtpResponse(maskedPhone, otp);
        }
    }
}

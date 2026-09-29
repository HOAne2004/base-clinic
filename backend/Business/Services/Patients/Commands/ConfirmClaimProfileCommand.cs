using BaseClinic.Business.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BaseClinic.Business.Services.Patients.Commands
{
    public record ConfirmClaimProfileCommand(
        Guid AccountId,    // ID của tài khoản B (từ JWT)
        string PatientCode,
        string Otp
    ) : IRequest<bool>;

    public class ConfirmClaimProfileCommandHandler : IRequestHandler<ConfirmClaimProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _patientRepository;
        private readonly IMemoryCache _memoryCache;

        public ConfirmClaimProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IPatientRepository patientRepository,
            IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _patientRepository = patientRepository;
            _memoryCache = memoryCache;
        }

        public async Task<bool> Handle(ConfirmClaimProfileCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra OTP trong Cache
            string cacheKey = $"ClaimProfile_OTP_{request.PatientCode}";

            if (!_memoryCache.TryGetValue(cacheKey, out string? cachedOtp))
            {
                throw new InvalidOperationException("Mã OTP đã hết hạn hoặc không tồn tại.");
            }
            if (cachedOtp != request.Otp)
            {
                throw new InvalidOperationException("Mã OTP không chính xác.");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // 2. Lấy hồ sơ bệnh nhân
                var patient = await _patientRepository.GetByPatientCodeAsync(request.PatientCode, cancellationToken)
                    ?? throw new InvalidOperationException("Mã bệnh nhân không tồn tại.");

                if (patient.AccountId != null)
                {
                    throw new InvalidOperationException("Hồ sơ này đã được liên kết với tài khoản khác.");
                }

                // 3. Gắn AccountId của B vào hồ sơ gốc (Sử dụng hàm đã tạo trong Entity)
                patient.ClaimProfile(request.AccountId);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // 4. Xóa OTP khỏi Cache để tránh tái sử dụng (Replay Attack)
                _memoryCache.Remove(cacheKey);

                return true;
            }
            catch (Exception ex)
            {
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw new Exception("Lỗi: ", ex);
                }
            }
        }
    }
}
    


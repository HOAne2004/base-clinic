using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// B (chủ hồ sơ) phản hồi yêu cầu theo dõi từ A.
    /// 
    /// LUỒNG:
    /// 1. Tìm delegation theo Id.
    /// 2. Check delegation đang Pending.
    /// 3. Lấy Patient primary của B (từ AccountId).
    /// 4. Check delegation.TargetPatientId == B's patient (bảo mật).
    /// 5. Approve hoặc Reject.
    /// </summary>
    public record RespondToDelegationCommand(
        Guid DelegationId,
        Guid AccountId,
        bool IsAccepted
    ) : IRequest<bool>;

    public class RespondToDelegationCommandHandler
        : IRequestHandler<RespondToDelegationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _patientRepo;
        private readonly IPatientDelegationRepository _delegationRepo;

        public RespondToDelegationCommandHandler(
            IUnitOfWork unitOfWork,
            IPatientRepository patientRepo,
            IPatientDelegationRepository delegationRepo)
        {
            _unitOfWork = unitOfWork;
            _patientRepo = patientRepo;
            _delegationRepo = delegationRepo;
        }

        public async Task<bool> Handle(
            RespondToDelegationCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm delegation ====================
            var delegation = await _delegationRepo.GetByIdAsync(
                request.DelegationId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy yêu cầu liên kết này.");

            if (delegation.Status != DelegationStatus.Pending)
                throw new DomainException("Yêu cầu này đã được xử lý trước đó.");

            // ==================== BƯỚC 2: Lấy Patient primary của B ====================
            var currentPatient = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken)
                ?? throw new DomainException(
                    "Tài khoản của bạn chưa có hồ sơ bệnh nhân gốc.");

            // ==================== BƯỚC 3: Bảo mật — Check ownership ====================
            if (delegation.TargetPatientId != currentPatient.Id)
                throw new ForbiddenException(
                    "Bạn không có quyền xử lý yêu cầu liên kết của người khác.");

            // ==================== BƯỚC 4: Approve hoặc Reject ====================
            if (request.IsAccepted)
            {
                delegation.Approve(request.AccountId);
            }
            else
            {
                delegation.Reject(request.AccountId, "Người nhận từ chối.");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
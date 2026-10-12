using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Bỏ liên kết dependent với Patient primary.
    /// 
    /// LUỒNG:
    /// 1. Tìm Patient primary của Account.
    /// 2. Tìm dependent.
    /// 3. Check ownership.
    /// 4. Unlink (PrimaryPatientId = null, RelationshipType = null).
    /// 5. Revoke delegation của owner (nếu có).
    /// 
    /// LƯU Ý:
    /// - KHÔNG xóa Patient — chỉ unlink.
    /// - Dependent trở thành Patient độc lập.
    /// - Dữ liệu lịch sử khám vẫn giữ nguyên.
    /// </summary>
    public record UnlinkDependentCommand(
        Guid AccountId,
        Guid DependentId
    ) : IRequest<Unit>;

    public class UnlinkDependentCommandHandler
        : IRequestHandler<UnlinkDependentCommand, Unit>
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IPatientDelegationRepository _delegationRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UnlinkDependentCommandHandler(
            IPatientRepository patientRepo,
            IPatientDelegationRepository delegationRepo,
            IUnitOfWork unitOfWork)
        {
            _patientRepo = patientRepo;
            _delegationRepo = delegationRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            UnlinkDependentCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm Patient primary ====================
            var primary = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken);

            if (primary == null)
                throw new DomainException("Tài khoản này chưa có hồ sơ bệnh nhân.");

            // ==================== BƯỚC 2: Tìm dependent ====================
            var dependent = await _patientRepo.GetByIdAsync(
                request.DependentId, cancellationToken);

            if (dependent == null)
                throw new NotFoundException("Không tìm thấy hồ sơ người phụ thuộc.");

            // ==================== BƯỚC 3: Check ownership ====================
            if (dependent.PrimaryPatientId != primary.Id)
                throw new DomainException(
                    "Hồ sơ này không thuộc quyền quản lý của bạn.");

            // ==================== BƯỚC 4: Unlink ====================
            dependent.RemovePrimaryPatient();
            // ⚡ RemovePrimaryPatient() cần set cả RelationshipType = null
            // (đã có logic trong Patient.SetPrimaryPatient/RemovePrimaryPatient nếu bạn đã sửa)

            _patientRepo.Update(dependent);

            // ==================== BƯỚC 5: Revoke auto-delegation của owner ====================
            var ownerDelegation = await _delegationRepo
                .GetActiveByTargetAndObserverAsync(
                    dependent.Id, request.AccountId, cancellationToken);

            if (ownerDelegation != null)
            {
                ownerDelegation.Revoke("Owner đã bỏ liên kết với dependent.");
            }

            // ==================== BƯỚC 6: Lưu ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
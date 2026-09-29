using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Enums;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    public record RevokeDelegationCommand(
        Guid DelegationId,
        Guid AccountId // ID của người đang thực hiện hành động (từ JWT)
    ) : IRequest<bool>;

    public class RevokeDelegationCommandHandler : IRequestHandler<RevokeDelegationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientDelegationRepository _delegationRepository;
        private readonly IPatientRepository _patientRepository;

        public RevokeDelegationCommandHandler(
            IUnitOfWork unitOfWork,
            IPatientDelegationRepository delegationRepository,
            IPatientRepository patientRepository)
        {
            _unitOfWork = unitOfWork;
            _delegationRepository = delegationRepository;
            _patientRepository = patientRepository;
        }
        public async Task<bool> Handle(RevokeDelegationCommand request, CancellationToken cancellationToken)
        {
            var delegation = await _delegationRepository.GetByIdAsync(request.DelegationId, cancellationToken)
                ?? throw new InvalidOperationException("Không tìm thấy liên kết này.");

            // Lấy hồ sơ gốc của người đang thao tác
            var currentPatient = await _patientRepository.GetPrimaryPatientByAccountIdAsync(request.AccountId, cancellationToken);

            // Kiểm tra quyền: Chỉ chủ hồ sơ (Target) HOẶC người theo dõi (Observer) mới được quyền xóa liên kết này
            bool isOwner = currentPatient != null && delegation.TargetPatientId == currentPatient.Id;
            bool isObserver = delegation.ObserverAccountId == request.AccountId;

            if (!isOwner && !isObserver)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thu hồi liên kết này.");
            }

            // Đổi trạng thái thành Revoked (hoặc bạn có thể gọi _delegationRepository.Remove(delegation) để xóa cứng)
            delegation.ChangeStatus(DelegationStatus.Revoked);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

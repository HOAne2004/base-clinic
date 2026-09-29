using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Enums;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    public record RespondToDelegationCommand(
        Guid DelegationId, // ID của lời mời liên kết
        Guid AccountId,    // AccountId của người đang thao tác (Tài khoản B)
        bool IsAccepted    // True = Đồng ý, False = Từ chối
    ) : IRequest<bool>;

    public class RespondToDelegationCommandHandler : IRequestHandler<RespondToDelegationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientDelegationRepository _delegationRepository;

        public RespondToDelegationCommandHandler(
            IUnitOfWork unitOfWork,
            IPatientRepository patientRepository,
            IPatientDelegationRepository delegationRepository)
        {
            _unitOfWork = unitOfWork;
            _patientRepository = patientRepository;
            _delegationRepository = delegationRepository;
        }

        public async Task<bool> Handle(RespondToDelegationCommand request, CancellationToken cancellationToken)
        {
            var delegation = await _delegationRepository.GetByIdAsync(request.DelegationId, cancellationToken)
                ?? throw new InvalidOperationException("Không tìm thấy yêu cầu liên kết này.");

            if (delegation.Status != DelegationStatus.Pending)
            {
                throw new InvalidOperationException("Yêu cầu này đã được xử lý trước đó.");
            }

            // 2. Lấy hồ sơ gốc của người đang thao tác (B)
            var currentPatient = await _patientRepository.GetPrimaryPatientByAccountIdAsync(request.AccountId, cancellationToken)
                ?? throw new InvalidOperationException("Tài khoản của bạn chưa có hồ sơ bệnh nhân gốc.");

            // 3. Xác thực bảo mật: Lời mời này có đúng là gửi cho hồ sơ của B không?
            if (delegation.TargetPatientId != currentPatient.Id)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xử lý yêu cầu liên kết của người khác.");
            }

            // 4. Cập nhật trạng thái
            var newStatus = request.IsAccepted ? DelegationStatus.Accepted : DelegationStatus.Rejected;
            delegation.ChangeStatus(newStatus); // Hàm ChangeStatus đã tạo trong Entity

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}

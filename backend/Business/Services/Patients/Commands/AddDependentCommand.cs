using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Domain.Extensions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Thêm người thân (dependent) cho Patient primary hiện tại.
    /// 
    /// 2 LUỒNG:
    /// 
    /// LUỒNG A — Người thân ĐÃ CÓ Account:
    ///     → Đây KHÔNG phải tạo dependent.
    ///     → Đây là "request theo dõi" hồ sơ của B.
    ///     → Tạo PatientDelegation status = Pending → chờ B approve.
    /// 
    /// LUỒNG B — Người thân CHƯA CÓ Account:
    ///     → Tạo Patient mới (dependent).
    ///     → PrimaryPatientId = Patient của A.
    ///     → Auto-tạo PatientDelegation status = Accepted (Full access).
    /// </summary>
    public record AddDependentCommand(
        Guid AccountId,
        string FullName,
        string PhoneNumber,
        DateTime? DateOfBirth,
        bool? Gender,
        PatientRelationshipType RelationshipType
    ) : IRequest<string>;

    public class AddDependentCommandHandler
        : IRequestHandler<AddDependentCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IPatientDelegationRepository _delegationRepo;
        private readonly IPatientCodeGenerator _patientCodeGenerator;

        public AddDependentCommandHandler(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepo,
            IPatientRepository patientRepo,
            IPatientDelegationRepository delegationRepo,
            IPatientCodeGenerator patientCodeGenerator)
        {
            _unitOfWork = unitOfWork;
            _accountRepo = accountRepo;
            _patientRepo = patientRepo;
            _delegationRepo = delegationRepo;
            _patientCodeGenerator = patientCodeGenerator;
        }

        public async Task<string> Handle(
            AddDependentCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Lấy Patient primary của A ====================
            var primaryPatient = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken)
                ?? throw new DomainException(
                    "Tài khoản của bạn chưa có hồ sơ bệnh nhân gốc.");

            // ==================== BƯỚC 2: Check SĐT người thân ====================
            var existingAccount = await _accountRepo.GetByPhoneNumberAsync(
                request.PhoneNumber, cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                if (existingAccount != null)
                {
                    // ==================== LUỒNG A: Người thân ĐÃ CÓ Account ====================
                    // → Đây là request theo dõi, không phải tạo dependent.

                    // Lấy Patient primary của B
                    var targetPatient = await _patientRepo.GetPrimaryByAccountIdAsync(
                        existingAccount.Id, cancellationToken)
                        ?? throw new DomainException(
                            "Tài khoản người thân bị lỗi dữ liệu (không tìm thấy hồ sơ gốc).");

                    // Check đã có delegation chưa
                    var existingDelegation = await _delegationRepo
                        .GetActiveByTargetAndObserverAsync(
                            targetPatient.Id, request.AccountId, cancellationToken);

                    if (existingDelegation != null)
                        throw new DomainException("Yêu cầu đã được gửi trước đó.");

                    // Tạo delegation Pending — chờ B approve
                    var delegation = PatientDelegation.CreateRequest(
                        targetPatientId: targetPatient.Id,
                        requesterAccountId: request.AccountId,
                        relationshipType: request.RelationshipType);

                    _delegationRepo.Add(delegation);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return $"Đã gửi yêu cầu liên kết đến số điện thoại {request.PhoneNumber}. " +
                           $"Vui lòng chờ xác nhận.";
                }
                else
                {
                    // ==================== LUỒNG B: Người thân CHƯA CÓ Account ====================
                    // → Tạo Patient dependent + auto delegation Accepted.

                    string patientCode = await _patientCodeGenerator
                        .GenerateAsync(cancellationToken);

                    // Tạo Patient với PrimaryPatientId + RelationshipType
                    var newDependent = new Patient(
                        patientCode: patientCode,
                        fullName: request.FullName,
                        accountId: null,
                        primaryPatientId: primaryPatient.Id,
                        isPrimary: false,
                        relationshipType: request.RelationshipType);

                    // Cập nhật profile (signature mới)
                    newDependent.UpdateProfile(
                        fullName: request.FullName,
                        avatarUrl: null,
                        address: null,
                        identityNumber: null,
                        gender: request.Gender,
                        dateOfBirth: request.DateOfBirth,
                        healthInsuranceNumber: null);

                    _patientRepo.Add(newDependent);

                    // Save để có Id cho newDependent
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Auto-tạo delegation Accepted (Owner delegation)
                    var delegation = PatientDelegation.CreateOwnerDelegation(
                        targetPatientId: newDependent.Id,
                        ownerAccountId: request.AccountId,
                        relationshipType: request.RelationshipType.Invert());

                    _delegationRepo.Add(delegation);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return "Thêm hồ sơ người thân thành công. " +
                           "Bạn đã có thể đặt lịch cho hồ sơ này.";
                }
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
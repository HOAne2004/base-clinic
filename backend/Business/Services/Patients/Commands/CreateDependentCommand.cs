using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Domain.Extensions;
using BaseClinic.Domain.ValueObjects;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Tạo dependent cho Patient primary hiện tại.
    /// 
    /// LUỒNG:
    /// 1. Tìm Patient primary của Account.
    /// 2. Sinh PatientCode.
    /// 3. Tạo Patient dependent (IsPrimary = false, PrimaryPatientId = primary.Id).
    /// 4. Tạo PatientDelegation auto (Owner delegation với Full access).
    /// 
    /// LƯU Ý VỀ RELATIONSHIP TYPE:
    /// - Patient.RelationshipType = quan hệ của B với A (VD: Child).
    /// - PatientDelegation.RelationshipType = quan hệ của A với B (VD: Parent).
    /// → Cần đảo ngược qua `Invert()`.
    /// </summary>
    public record CreateDependentCommand(
        Guid AccountId,
        CreateDependentDto Dto
    ) : IRequest<CreateDependentResultDto>;

    public class CreateDependentCommandHandler
        : IRequestHandler<CreateDependentCommand, CreateDependentResultDto>
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IPatientDelegationRepository _delegationRepo;
        private readonly IPatientCodeGenerator _codeGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDependentCommandHandler(
            IPatientRepository patientRepo,
            IPatientDelegationRepository delegationRepo,
            IPatientCodeGenerator codeGenerator,
            IUnitOfWork unitOfWork)
        {
            _patientRepo = patientRepo;
            _delegationRepo = delegationRepo;
            _codeGenerator = codeGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateDependentResultDto> Handle(
            CreateDependentCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Tìm Patient primary ====================
            var primary = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken);

            if (primary == null)
                throw new DomainException(
                    "Tài khoản này chưa có hồ sơ bệnh nhân.");

            var dto = request.Dto;

            // ==================== BƯỚC 2: Validate RelationshipType ====================
            if (dto.RelationshipType == Domain.Enums.PatientRelationshipType.Self)
                throw new DomainException(
                    "Không thể tạo dependent với quan hệ 'Self'. " +
                    "Vui lòng chọn quan hệ khác.");

            // ==================== BƯỚC 3: Sinh mã BN ====================
            var code = await _codeGenerator.GenerateAsync(cancellationToken);

            // ==================== BƯỚC 4: Tạo Patient dependent ====================
            var dependent = new Patient(
                patientCode: code,
                fullName: dto.FullName,
                accountId: null,
                primaryPatientId: primary.Id,
                isPrimary: false,
                relationshipType: dto.RelationshipType);

            dependent.UpdateProfile(
                fullName: dto.FullName,
                avatarUrl: null,
                address: MapAddress(dto.Address),
                identityNumber: dto.IdentityNumber,
                gender: dto.Gender,
                dateOfBirth: dto.DateOfBirth,
                healthInsuranceNumber: dto.HealthInsuranceNumber);

            _patientRepo.Add(dependent);

            // ==================== BƯỚC 5: Tạo Auto-Delegation ====================
            // ⚡ Đảo ngược relationship: 
            //   Patient.RelationshipType = "B là con của A" (Child)
            //   Delegation.RelationshipType = "A là mẹ của B" (Parent)
            var delegationRelationship = dto.RelationshipType.Invert();

            var delegation = PatientDelegation.CreateOwnerDelegation(
                targetPatientId: dependent.Id,
                ownerAccountId: request.AccountId,
                relationshipType: delegationRelationship);

            _delegationRepo.Add(delegation);

            // ==================== BƯỚC 6: Lưu ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ==================== BƯỚC 7: Response ====================
            return new CreateDependentResultDto(
                DependentId: dependent.Id,
                PatientCode: dependent.PatientCode);
        }

        private static Address? MapAddress(AddressDto? dto)
        {
            if (dto == null) return null;
            return new Address(dto.Tinh, dto.Xa, dto.Duong, dto.SoNha);
        }
    }
}
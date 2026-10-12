using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Exceptions;
using MediatR;
using System.Text.Json;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Merge 2 Patient thành 1.
    /// 
    /// QUY TẮC:
    /// - Chỉ Admin được gọi.
    /// - Survivor = Patient có Account. Nếu cả 2 có Account → throw.
    /// - Nếu cả 2 không Account → chọn Patient cũ hơn.
    /// - Di chuyển toàn bộ dữ liệu từ merged → survivor.
    /// - Merged bị soft delete + đánh dấu MergedIntoPatientId.
    /// - Ghi log PatientMergeLog.
    /// </summary>
    public record MergePatientCommand(
        Guid SurvivorPatientId,
        Guid MergedPatientId,
        string Reason,
        Guid MergedByAccountId,
        bool ConfirmNoIdentity = false
    ) : IRequest<MergePatientResultDto>;

    public class MergePatientCommandHandler
        : IRequestHandler<MergePatientCommand, MergePatientResultDto>
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IEncounterRepository _encounterRepo;
        private readonly IPatientDelegationRepository _delegationRepo;
        private readonly IRepository<PatientMergeLog> _mergeLogRepo;
        private readonly IUnitOfWork _unitOfWork;

        public MergePatientCommandHandler(
            IPatientRepository patientRepo,
            IAppointmentRepository appointmentRepo,
            IEncounterRepository encounterRepo,
            IPatientDelegationRepository delegationRepo,
            IRepository<PatientMergeLog> mergeLogRepo,
            IUnitOfWork unitOfWork)
        {
            _patientRepo = patientRepo;
            _appointmentRepo = appointmentRepo;
            _encounterRepo = encounterRepo;
            _delegationRepo = delegationRepo;
            _mergeLogRepo = mergeLogRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<MergePatientResultDto> Handle(
            MergePatientCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Validate cơ bản ====================
            if (request.SurvivorPatientId == request.MergedPatientId)
                throw new DomainException("Không thể merge Patient vào chính nó.");

            var survivor = await _patientRepo.GetByIdWithDependentsAsync(
                request.SurvivorPatientId, cancellationToken);

            var merged = await _patientRepo.GetByIdWithDependentsAsync(
                request.MergedPatientId, cancellationToken);

            if (survivor == null)
                throw new NotFoundException("Không tìm thấy Patient survivor.");

            if (merged == null)
                throw new NotFoundException("Không tìm thấy Patient merged.");

            // ==================== BƯỚC 2: Validate trạng thái ====================
            if (survivor.MergedIntoPatientId.HasValue)
                throw new DomainException(
                    "Survivor Patient đã bị merge trước đó. Không thể làm survivor.");

            if (merged.MergedIntoPatientId.HasValue)
                throw new DomainException(
                    "Merged Patient đã bị merge trước đó.");

            // ==================== BƯỚC 3: Validate Account ====================
            if (survivor.AccountId.HasValue && merged.AccountId.HasValue)
                throw new DomainException(
                    "Cả 2 Patient đều có Account. Không thể tự động merge. " +
                    "Vui lòng liên hệ Admin để xử lý thủ công.");

            // Nếu survivor không có Account nhưng merged có → swap
            // (vì Patient có Account nên làm survivor)
            if (!survivor.AccountId.HasValue && merged.AccountId.HasValue)
            {
                // Swap — survivor có Account hơn
                var temp = survivor;
                survivor = merged;
                merged = temp;
            }

            // ==================== BƯỚC 4: Validate quan hệ dependent ====================
            bool survivorIsDependent = survivor.PrimaryPatientId.HasValue;
            bool mergedIsDependent = merged.PrimaryPatientId.HasValue;

            // Chặn: Survivor là dependent, merged là primary độc lập
            if (survivorIsDependent && !mergedIsDependent)
                throw new DomainException(
                    "Không thể merge Patient độc lập vào Patient phụ thuộc. " +
                    "Vui lòng chọn survivor là Patient độc lập.");

            // Chặn: Cả 2 là dependent nhưng khác primary
            if (survivorIsDependent && mergedIsDependent
                && survivor.PrimaryPatientId != merged.PrimaryPatientId)
                throw new DomainException(
                    "Không thể merge 2 Patient phụ thuộc vào 2 Patient chính khác nhau.");

            // ==================== BƯỚC 5: Validate CCCD ====================
            bool survivorHasCccd = !string.IsNullOrWhiteSpace(survivor.IdentityNumber);
            bool mergedHasCccd = !string.IsNullOrWhiteSpace(merged.IdentityNumber);

            if (survivorHasCccd && mergedHasCccd)
            {
                // Cả 2 có CCCD → phải giống nhau
                if (survivor.IdentityNumber != merged.IdentityNumber)
                    throw new DomainException(
                        $"CCCD không khớp: survivor ({survivor.IdentityNumber}) " +
                        $"vs merged ({merged.IdentityNumber}).");
            }
            else if (!survivorHasCccd && !mergedHasCccd)
            {
                // Cả 2 đều không CCCD → cần Admin confirm
                if (!request.ConfirmNoIdentity)
                    throw new DomainException(
                        "Cả 2 Patient đều chưa có CCCD. " +
                        "Vui lòng xác nhận đã kiểm tra thông tin thủ công.");
            }
            else
            {
                // Chỉ 1 có CCCD → cần Admin confirm
                if (!request.ConfirmNoIdentity)
                    throw new DomainException(
                        "Chỉ 1 trong 2 Patient có CCCD. " +
                        "Vui lòng xác nhận đã kiểm tra thông tin thủ công.");
            }

            // ==================== BƯỚC 6: Lấy danh sách records sẽ chuyển ====================
            var appointments = _appointmentRepo
                .GetQueryableWithTracking()
                .Where(a => a.PatientId == merged.Id)
                .ToList();

            var encounters = _encounterRepo
                .GetQueryableWithTracking()
                .Where(e => e.PatientId == merged.Id)
                .ToList();

            var delegations = _delegationRepo
                .GetQueryableWithTracking()
                .Where(d => d.TargetPatientId == merged.Id)
                .ToList();

            var dependents = merged.Dependents.ToList();

            var mergedAt = DateTimeOffset.UtcNow;

            // ==================== BƯỚC 7: Snapshot dữ liệu ====================
            var snapshot = BuildFullSnapshot(
                survivor, merged, appointments, encounters, delegations, dependents, mergedAt);

            // ==================== BƯỚC 8: Di chuyển dữ liệu (dùng list đã có) ====================
            foreach (var appt in appointments)
                appt.SetPatientIdInternal(survivor.Id);

            foreach (var enc in encounters)
                enc.SetPatientIdInternal(survivor.Id);

            foreach (var del in delegations)
                del.SetTargetPatientIdInternal(survivor.Id);

            foreach (var dep in dependents)
                dep.SetPrimaryPatientInternal(survivor.Id);

            // Account move
            bool accountMoved = false;
            if (!survivor.AccountId.HasValue && merged.AccountId.HasValue)
            {
                survivor.SetAccountIdInternal(merged.AccountId.Value);
                merged.SetAccountIdInternal(null);
                accountMoved = true;
            }

            // ==================== BƯỚC 9: Đánh dấu merged ====================
            merged.MarkAsMerged(survivor.Id);

            // ==================== BƯỚC 10: Ghi log ====================
            var mergeLog = new PatientMergeLog(
                survivorPatientId: survivor.Id,
                mergedPatientId: merged.Id,
                mergedSnapshot: snapshot,
                reason: request.Reason,
                mergedByAccountId: request.MergedByAccountId);

            _mergeLogRepo.Add(mergeLog);

            // ==================== BƯỚC 11: Save ====================
            _patientRepo.Update(survivor);
            _patientRepo.Update(merged);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ==================== BƯỚC 12: Response ====================
            return new MergePatientResultDto(
                SurvivorPatientId: survivor.Id,
                SurvivorPatientCode: survivor.PatientCode,
                MergedPatientId: merged.Id,
                MergedPatientCode: merged.PatientCode,
                AppointmentsMoved: appointments.Count,
                EncountersMoved: encounters.Count,
                DelegationsMoved: delegations.Count,
                DependentsMoved: dependents.Count,
                AccountMoved: accountMoved);
        }

        /// <summary>
        /// Xây dựng JSON snapshot đầy đủ cho rollback.
        /// Bao gồm: merged patient info + toàn bộ records đã chuyển.
        /// </summary>
        private static string BuildFullSnapshot(
            Patient survivor,
            Patient merged,
            List<Appointment> appointments,
            List<Encounter> encounters,
            List<PatientDelegation> delegations,
            List<Patient> dependents,
            DateTimeOffset mergedAt)
        {
            var snapshot = new
            {
                mergedAt = mergedAt.ToString("O"),
                survivor = new
                {
                    id = survivor.Id,
                    patientCode = survivor.PatientCode,
                    fullName = survivor.FullName,
                    accountId = survivor.AccountId,
                    primaryPatientId = survivor.PrimaryPatientId,
                    isPrimary = survivor.IsPrimary
                },
                merged = new
                {
                    id = merged.Id,
                    patientCode = merged.PatientCode,
                    fullName = merged.FullName,
                    accountId = merged.AccountId,
                    primaryPatientId = merged.PrimaryPatientId,
                    isPrimary = merged.IsPrimary,
                    identityNumber = merged.IdentityNumber,
                    healthInsuranceNumber = merged.HealthInsuranceNumber,
                    gender = merged.Gender,
                    dateOfBirth = merged.DateOfBirth?.ToString("O"),
                    avatarUrl = merged.AvatarUrl,
                    address = new
                    {
                        tinh = merged.Address.Tinh,
                        xa = merged.Address.Xa,
                        duong = merged.Address.Duong,
                        soNha = merged.Address.SoNha
                    }
                },
                transferredRecords = new
                {
                    appointments = appointments.Select(a => new
                    {
                        id = a.Id,
                        appointmentCode = a.AppointmentCode,
                        oldPatientId = merged.Id,
                        newPatientId = survivor.Id,
                        createdDate = a.CreatedDate.ToString("O")
                    }).ToList(),
                    encounters = encounters.Select(e => new
                    {
                        id = e.Id,
                        oldPatientId = merged.Id,
                        newPatientId = survivor.Id,
                        createdDate = e.CreatedDate.ToString("O")
                    }).ToList(),
                    delegations = delegations.Select(d => new
                    {
                        id = d.Id,
                        oldTargetPatientId = merged.Id,
                        newTargetPatientId = survivor.Id,
                        createdDate = d.CreatedDate.ToString("O")
                    }).ToList(),
                    dependents = dependents.Select(dep => new
                    {
                        id = dep.Id,
                        patientCode = dep.PatientCode,
                        oldPrimaryPatientId = merged.Id,
                        newPrimaryPatientId = survivor.Id,
                        createdDate = dep.CreatedDate.ToString("O")
                    }).ToList(),
                    accountMoved = new
                    {
                        fromPatientId = merged.Id,
                        toPatientId = survivor.Id,
                        accountId = merged.AccountId
                    }
                }
            };

            return JsonSerializer.Serialize(snapshot,
                new JsonSerializerOptions { WriteIndented = false });
        }
    }
}
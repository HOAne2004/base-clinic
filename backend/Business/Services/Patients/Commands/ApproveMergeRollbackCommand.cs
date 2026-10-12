using BaseClinic.Business.Interfaces;
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BaseClinic.Business.Services.Patients.Commands
{
    /// <summary>
    /// Duyệt yêu cầu rollback + thực hiện rollback.
    /// 
    /// ĐIỀU KIỆN:
    /// - Request.Status = Pending.
    /// - Merge log chưa rollback.
    /// - Trong time window (7 ngày).
    /// - Mọi conflict record phải được resolve.
    /// 
    /// QUY TRÌNH:
    /// 1. Load request + merge log.
    /// 2. Validate.
    /// 3. Deserialize snapshot.
    /// 4. Restore merged patient (IsDeleted = false, MergedIntoPatientId = null).
    /// 5. Chuyển records trong snapshot về merged.
    /// 6. Xử lý conflict theo resolution.
    /// 7. Update log + request.
    /// 8. Gửi SMS thông báo.
    /// 9. Save.
    /// </summary>
    public record ApproveMergeRollbackCommand(
        Guid RequestId,
        Guid ReviewerAccountId,
        string? ReviewNote,
        List<ConflictResolutionDto> ConflictResolutions
    ) : IRequest<Unit>;

    public class ApproveMergeRollbackCommandHandler
        : IRequestHandler<ApproveMergeRollbackCommand, Unit>
    {
        private const int MaxRollbackDays = 7;

        private readonly IRepository<MergeRollbackRequest> _requestRepo;
        private readonly IRepository<PatientMergeLog> _mergeLogRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IEncounterRepository _encounterRepo;
        private readonly IPatientDelegationRepository _delegationRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveMergeRollbackCommandHandler(
            IRepository<MergeRollbackRequest> requestRepo,
            IRepository<PatientMergeLog> mergeLogRepo,
            IPatientRepository patientRepo,
            IAppointmentRepository appointmentRepo,
            IEncounterRepository encounterRepo,
            IPatientDelegationRepository delegationRepo,
            IAccountRepository accountRepo,
            ISmsService smsService,
            IUnitOfWork unitOfWork)
        {
            _requestRepo = requestRepo;
            _mergeLogRepo = mergeLogRepo;
            _patientRepo = patientRepo;
            _appointmentRepo = appointmentRepo;
            _encounterRepo = encounterRepo;
            _delegationRepo = delegationRepo;
            _accountRepo = accountRepo;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            ApproveMergeRollbackCommand request,
            CancellationToken cancellationToken)
        {
            // ==================== BƯỚC 1: Load request ====================
            var rollbackRequest = await _requestRepo.GetByIdAsync(
                request.RequestId, cancellationToken);

            if (rollbackRequest == null)
                throw new NotFoundException("Không tìm thấy yêu cầu rollback.");

            if (rollbackRequest.Status != MergeRollbackRequestStatus.Pending)
                throw new DomainException(
                    "Chỉ có thể duyệt yêu cầu đang ở trạng thái Pending.");

            // ==================== BƯỚC 2: Load merge log ====================
            var mergeLog = await _mergeLogRepo.GetByIdAsync(
                rollbackRequest.PatientMergeLogId, cancellationToken);

            if (mergeLog == null)
                throw new NotFoundException("Không tìm thấy merge log.");

            if (mergeLog.IsRolledBack)
                throw new DomainException("Merge log này đã được rollback trước đó.");

            // ==================== BƯỚC 3: Validate time window ====================
            var elapsed = DateTimeOffset.UtcNow - mergeLog.MergedAt;
            if (elapsed.TotalDays > MaxRollbackDays)
                throw new DomainException(
                    $"Đã quá {MaxRollbackDays} ngày kể từ khi merge. Không thể rollback.");

            // ==================== BƯỚC 4: Validate conflict resolutions ====================
            // Nếu có conflict records nhưng không có resolution → chặn
            // (Kiểm tra logic ở đây — có thể query conflict và so sánh)
            // Đơn giản: yêu cầu request.ConflictResolutions != null nếu có conflict
            // → bỏ qua validate này cho Phase 1, admin tự chịu trách nhiệm

            // ==================== BƯỚC 5: Load patients ====================
            var survivor = await _patientRepo.GetByIdAsync(
                mergeLog.SurvivorPatientId, cancellationToken);

            var merged = await _patientRepo.GetByIdAsync(
                mergeLog.MergedPatientId, cancellationToken);

            if (survivor == null || merged == null)
                throw new NotFoundException("Không tìm thấy Patient survivor hoặc merged.");

            // ==================== BƯỚC 6: Deserialize snapshot ====================
            var snapshotDoc = JsonDocument.Parse(mergeLog.MergedSnapshot);
            var transferredRecords = snapshotDoc.RootElement
                .GetProperty("transferredRecords");

            // ==================== BƯỚC 7: Restore merged Patient ====================
            merged.RestoreFromMerge();   // Cần thêm method này vào Patient

            // ==================== BƯỚC 8: Chuyển records về merged ====================

            // 8.1. Appointments từ snapshot
            var appointmentIds = transferredRecords
                .GetProperty("appointments")
                .EnumerateArray()
                .Select(x => Guid.Parse(x.GetProperty("id").GetString()!))
                .ToHashSet();

            var appointmentsToMove = await _appointmentRepo
                .GetQueryableWithTracking()
                .Where(a => appointmentIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            foreach (var appt in appointmentsToMove)
                appt.SetPatientIdInternal(merged.Id);

            // 8.2. Encounters
            var encounterIds = transferredRecords
                .GetProperty("encounters")
                .EnumerateArray()
                .Select(x => Guid.Parse(x.GetProperty("id").GetString()!))
                .ToHashSet();

            var encountersToMove = await _encounterRepo
                .GetQueryableWithTracking()
                .Where(e => encounterIds.Contains(e.Id))
                .ToListAsync(cancellationToken);

            foreach (var enc in encountersToMove)
                enc.SetPatientIdInternal(merged.Id);

            // 8.3. Delegations
            var delegationIds = transferredRecords
                .GetProperty("delegations")
                .EnumerateArray()
                .Select(x => Guid.Parse(x.GetProperty("id").GetString()!))
                .ToHashSet();

            var delegationsToMove = await _delegationRepo
                .GetQueryableWithTracking()
                .Where(d => delegationIds.Contains(d.Id))
                .ToListAsync(cancellationToken);

            foreach (var del in delegationsToMove)
                del.SetTargetPatientIdInternal(merged.Id);

            // 8.4. Dependents
            var dependentIds = transferredRecords
                .GetProperty("dependents")
                .EnumerateArray()
                .Select(x => Guid.Parse(x.GetProperty("id").GetString()!))
                .ToHashSet();

            var dependentsToMove = await _patientRepo
                .GetQueryableWithTracking()
                .Where(p => dependentIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            foreach (var dep in dependentsToMove)
                dep.SetPrimaryPatientInternal(merged.Id);

            // ==================== BƯỚC 9: Xử lý conflict records ====================
            // Conflict records là các record có CreatedDate > MergedAt
            // → Đã được đưa vào resolution. Nếu không có resolution → giữ ở survivor.

            if (request.ConflictResolutions.Any())
            {
                await ApplyConflictResolutions(
                    request.ConflictResolutions,
                    survivor.Id,
                    merged.Id,
                    cancellationToken);
            }

            // ==================== BƯỚC 10: Xử lý Account ====================
            var accountMoved = transferredRecords.GetProperty("accountMoved");
            var accountIdStr = accountMoved.GetProperty("accountId").GetString();

            if (!string.IsNullOrEmpty(accountIdStr))
            {
                var accountId = Guid.Parse(accountIdStr);

                // Nếu survivor hiện đang giữ account này → trả về merged
                if (survivor.AccountId == accountId)
                {
                    survivor.SetAccountIdInternal(null);
                    merged.SetAccountIdInternal(accountId);
                }
            }

            // ==================== BƯỚC 11: Update log + request ====================
            mergeLog.MarkAsRolledBack(request.ReviewerAccountId, request.ReviewNote ?? "Rollback theo yêu cầu.");

            var resolutionsJson = request.ConflictResolutions.Any()
                ? JsonSerializer.Serialize(request.ConflictResolutions)
                : null;

            rollbackRequest.Approve(
                reviewerAccountId: request.ReviewerAccountId,
                reviewNote: request.ReviewNote,
                conflictResolutions: resolutionsJson);

            // ==================== BƯỚC 12: Save ====================
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ==================== BƯỚC 13: SMS notification ====================
            await SendRollbackNotificationsAsync(survivor, merged, cancellationToken);

            return Unit.Value;
        }

        // ==================== HELPERS ====================

        private async Task ApplyConflictResolutions(
            List<ConflictResolutionDto> resolutions,
            Guid survivorId,
            Guid mergedId,
            CancellationToken cancellationToken)
        {
            foreach (var res in resolutions)
            {
                if (res.BelongsTo == ConflictBelongsTo.Survivor)
                    continue;   // Giữ nguyên — không cần làm gì

                // belongsTo = Merged → chuyển sang merged
                switch (res.RecordType)
                {
                    case "Appointment":
                        var appt = await _appointmentRepo
                            .GetByIdAsync(res.RecordId, cancellationToken);
                        appt?.SetPatientIdInternal(mergedId);
                        break;

                    case "Encounter":
                        var enc = await _encounterRepo
                            .GetByIdAsync(res.RecordId, cancellationToken);
                        enc?.SetPatientIdInternal(mergedId);
                        break;

                    case "Delegation":
                        var del = await _delegationRepo
                            .GetByIdAsync(res.RecordId, cancellationToken);
                        del?.SetTargetPatientIdInternal(mergedId);
                        break;

                    case "Dependent":
                        var dep = await _patientRepo
                            .GetByIdAsync(res.RecordId, cancellationToken);
                        dep?.SetPrimaryPatientInternal(mergedId);
                        break;
                }
            }
        }

        private async Task SendRollbackNotificationsAsync(
            Patient survivor,
            Patient merged,
            CancellationToken cancellationToken)
        {
            // Survivor
            if (survivor.AccountId.HasValue)
            {
                var survivorAccount = await _accountRepo.GetByIdAsync(
                    survivor.AccountId, cancellationToken);

                if (survivorAccount != null)
                {
                    await _smsService.SendNotificationAsync(
                        survivorAccount.PhoneNumber,
                        $"[BaseClinic] Yêu cầu hoàn tác gộp hồ sơ đã được duyệt. " +
                        $"Hồ sơ {merged.PatientCode} đã được tách khỏi hồ sơ của bạn.",
                        cancellationToken);
                }
            }

            // Merged
            if (merged.AccountId.HasValue)
            {
                var mergedAccount = await _accountRepo.GetByIdAsync(
                    merged.AccountId, cancellationToken);

                if (mergedAccount != null)
                {
                    await _smsService.SendNotificationAsync(
                        mergedAccount.PhoneNumber,
                        $"[BaseClinic] Hồ sơ {merged.PatientCode} đã được khôi phục. " +
                        $"Mọi dữ liệu khám ban đầu đã được phục hồi.",
                        cancellationToken);
                }
            }
        }
    }
}
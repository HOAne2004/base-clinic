using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Queries
{
    /// <summary>
    /// Danh sách yêu cầu hủy đang Pending — cho Admin.
    /// </summary>
    public record GetPendingCancellationRequestsQuery()
        : IRequest<List<ScheduleCancellationRequestDto>>;

    public class GetPendingCancellationRequestsQueryHandler
        : IRequestHandler<GetPendingCancellationRequestsQuery, List<ScheduleCancellationRequestDto>>
    {
        private readonly IRepository<ScheduleCancellationRequest> _requestRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetPendingCancellationRequestsQueryHandler(
            IRepository<ScheduleCancellationRequest> requestRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _requestRepo = requestRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<List<ScheduleCancellationRequestDto>> Handle(
            GetPendingCancellationRequestsQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Load requests + schedule + doctor + accounts
            var requests = await _requestRepo.GetQueryable()
                .Include(r => r.DoctorSchedule)
                    .ThenInclude(s => s.Doctor)
                .Include(r => r.RequestedByAccount)
                .Include(r => r.ReviewedByAccount)
                .Where(r => r.Status == ScheduleCancellationRequestStatus.Pending)
                .OrderBy(r => r.CreatedDate)
                .ToListAsync(cancellationToken);

            if (!requests.Any())
                return new List<ScheduleCancellationRequestDto>();

            // 2. Batch load doctor account names
            var doctorAccountIds = requests
                .Select(r => r.DoctorSchedule.Doctor.AccountId)
                .Distinct()
                .ToList();

            var doctorNames = await _accountRepo.GetQueryable()
                .Where(a => doctorAccountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // 3. Batch load departments
            var departmentIds = requests
                .Select(r => r.DoctorSchedule.Doctor.DepartmentId)
                .Distinct()
                .ToList();

            var departments = await _departmentRepo.GetQueryable()
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d, cancellationToken);

            // 4. Map
            return requests
                .Select(r => ScheduleCancellationRequestMapper.ToDto(
                    request: r,
                    requesterName: r.RequestedByAccount?.FullName ?? string.Empty,
                    reviewerName: r.ReviewedByAccount?.FullName,
                    doctorName: doctorNames.GetValueOrDefault(
                        r.DoctorSchedule.Doctor.AccountId, string.Empty),
                    department: departments.GetValueOrDefault(
                        r.DoctorSchedule.Doctor.DepartmentId)))
                .ToList();
        }
    }
}
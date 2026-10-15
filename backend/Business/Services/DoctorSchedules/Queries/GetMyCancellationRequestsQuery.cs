using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.DoctorSchedules.Queries
{
    /// <summary>
    /// Yêu cầu hủy do tôi gửi — cho Doctor/Receptionist.
    /// </summary>
    public record GetMyCancellationRequestsQuery(Guid AccountId)
        : IRequest<List<ScheduleCancellationRequestDto>>;

    public class GetMyCancellationRequestsQueryHandler
        : IRequestHandler<GetMyCancellationRequestsQuery, List<ScheduleCancellationRequestDto>>
    {
        private readonly IRepository<ScheduleCancellationRequest> _requestRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetMyCancellationRequestsQueryHandler(
            IRepository<ScheduleCancellationRequest> requestRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _requestRepo = requestRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<List<ScheduleCancellationRequestDto>> Handle(
            GetMyCancellationRequestsQuery request,
            CancellationToken cancellationToken)
        {
            var requests = await _requestRepo.GetQueryable()
                .Include(r => r.DoctorSchedule)
                    .ThenInclude(s => s.Doctor)
                .Include(r => r.RequestedByAccount)
                .Include(r => r.ReviewedByAccount)
                .Where(r => r.RequestedByAccountId == request.AccountId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync(cancellationToken);

            if (!requests.Any())
                return new List<ScheduleCancellationRequestDto>();

            var doctorAccountIds = requests
                .Select(r => r.DoctorSchedule.Doctor.AccountId)
                .Distinct()
                .ToList();

            var doctorNames = await _accountRepo.GetQueryable()
                .Where(a => doctorAccountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            var departmentIds = requests
                .Select(r => r.DoctorSchedule.Doctor.DepartmentId)
                .Distinct()
                .ToList();

            var departments = await _departmentRepo.GetQueryable()
                .Where(d => departmentIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id, d => d, cancellationToken);

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
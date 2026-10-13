using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Doctor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    /// <summary>
    /// Danh sách bác sĩ đang active + available for online.
    /// Dùng cho dropdown chọn bác sĩ khi đặt lịch.
    /// 
    /// FILTER:
    /// - IsActive = true
    /// - IsAvailableForOnline = true
    /// - (Optional) DepartmentId
    /// 
    /// Không paging — số lượng nhỏ.
    /// </summary>
    public record GetActiveDoctorsQuery(Guid? DepartmentId = null)
        : IRequest<List<DoctorLookupDto>>;

    public class GetActiveDoctorsQueryHandler
        : IRequestHandler<GetActiveDoctorsQuery, List<DoctorLookupDto>>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;

        public GetActiveDoctorsQueryHandler(
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo)
        {
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
        }

        public async Task<List<DoctorLookupDto>> Handle(
            GetActiveDoctorsQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Query active doctors
            var query = _doctorRepo.GetQueryable()
                .Where(d => d.IsActive && d.IsAvailableForOnline);

            if (request.DepartmentId.HasValue)
                query = query.Where(d => d.DepartmentId == request.DepartmentId.Value);

            var doctors = await query
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.DoctorCode)
                .ToListAsync(cancellationToken);

            // 2. Batch load accounts
            var accountIds = doctors.Select(d => d.AccountId).Distinct().ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // 3. Map
            return doctors
                .Select(d => DoctorMapper.ToLookupDto(
                    d,
                    accountFullName: accountNames.GetValueOrDefault(d.AccountId, string.Empty)))
                .ToList();
        }
    }
}
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Doctor;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    /// <summary>
    /// Danh sách bác sĩ theo khoa — dùng cho Public (Guest/Patient).
    /// URL: GET /api/departments/{slug}/doctors
    /// 
    /// Chỉ trả về doctors active.
    /// Được gọi từ DepartmentController.
    /// </summary>
    public record GetDoctorsByDepartmentQuery(string DepartmentSlug)
        : IRequest<List<DoctorPublicListDto>>;

    public class GetDoctorsByDepartmentQueryHandler
        : IRequestHandler<GetDoctorsByDepartmentQuery, List<DoctorPublicListDto>>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;

        public GetDoctorsByDepartmentQueryHandler(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
        }

        public async Task<List<DoctorPublicListDto>> Handle(
            GetDoctorsByDepartmentQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Tìm department theo slug
            var department = await _departmentRepo.GetQueryable()
                .FirstOrDefaultAsync(d => d.Slug == request.DepartmentSlug, cancellationToken)
                ?? throw new NotFoundException(
                    $"Không tìm thấy khoa với slug '{request.DepartmentSlug}'.");

            // 2. Lấy doctors active
            var doctors = await _doctorRepo.GetQueryable()
                .Where(d => d.DepartmentId == department.Id && d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ThenBy(d => d.DoctorCode)
                .ToListAsync(cancellationToken);

            // 3. Batch load accounts
            var accountIds = doctors.Select(d => d.AccountId).Distinct().ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // 4. Map (department đã có sẵn)
            return doctors
                .Select(d => DoctorMapper.ToPublicListDto(
                    d,
                    accountFullName: accountNames.GetValueOrDefault(d.AccountId, string.Empty),
                    department: department))
                .ToList();
        }
    }
}
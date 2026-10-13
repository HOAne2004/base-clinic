using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Department;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Queries
{
    /// <summary>
    /// Lấy chi tiết khoa + danh sách doctors (bao gồm inactive) — cho Admin.
    /// </summary>
    public record GetDepartmentAdminDetailQuery(Guid Id)
        : IRequest<DepartmentAdminDetailDto>;

    public class GetDepartmentAdminDetailQueryHandler
        : IRequestHandler<GetDepartmentAdminDetailQuery, DepartmentAdminDetailDto>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;

        public GetDepartmentAdminDetailQueryHandler(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
        }

        public async Task<DepartmentAdminDetailDto> Handle(
            GetDepartmentAdminDetailQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Lấy department
            var dept = await _departmentRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy khoa với Id {request.Id}.");

            // 2. Lấy doctors (bao gồm cả IsActive = false)
            var doctors = await _doctorRepo.GetQueryable()
                .Where(d => d.DepartmentId == dept.Id)
                .OrderBy(d => d.SortOrder)
                .ToListAsync(cancellationToken);

            // 3. Batch load account names
            var accountIds = doctors.Select(d => d.AccountId).Distinct().ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // 4. Map
            return DepartmentMapper.ToAdminDetailDto(dept, doctors, accountNames);
        }
    }
}
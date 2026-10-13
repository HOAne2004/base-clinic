using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Department;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Departments.Queries
{
    /// <summary>
    /// Lấy chi tiết khoa công khai (chỉ doctors active).
    /// </summary>
    public record GetDepartmentPublicDetailQuery(Guid Id)
        : IRequest<DepartmentPublicDetailDto>;

    public class GetDepartmentPublicDetailQueryHandler
        : IRequestHandler<GetDepartmentPublicDetailQuery, DepartmentPublicDetailDto>
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;

        public GetDepartmentPublicDetailQueryHandler(
            IDepartmentRepository departmentRepo,
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo)
        {
            _departmentRepo = departmentRepo;
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
        }

        public async Task<DepartmentPublicDetailDto> Handle(
            GetDepartmentPublicDetailQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Lấy department — chỉ trả về nếu active
            var dept = await _departmentRepo.GetQueryable()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy khoa với Id {request.Id}.");

            if (!dept.IsActive)
                throw new NotFoundException($"Khoa này hiện không hoạt động.");

            // 2. Chỉ lấy doctors active
            var doctors = await _doctorRepo.GetQueryable()
                .Where(d => d.DepartmentId == dept.Id && d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ToListAsync(cancellationToken);

            // 3. Load account names
            var accountIds = doctors.Select(d => d.AccountId).Distinct().ToList();

            var accountNames = await _accountRepo.GetQueryable()
                .Where(a => accountIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName, cancellationToken);

            // 4. Map
            return DepartmentMapper.ToPublicDetailDto(dept, doctors, accountNames);
        }
    }
}
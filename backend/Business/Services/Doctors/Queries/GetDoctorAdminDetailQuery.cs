using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Doctor;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    /// <summary>
    /// Chi tiết bác sĩ cho Admin (đầy đủ thông tin).
    /// URL: GET /api/doctors/{id:guid}/admin
    /// </summary>
    public record GetDoctorAdminDetailQuery(Guid Id)
        : IRequest<DoctorAdminDetailDto>;

    public class GetDoctorAdminDetailQueryHandler
        : IRequestHandler<GetDoctorAdminDetailQuery, DoctorAdminDetailDto>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetDoctorAdminDetailQueryHandler(
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<DoctorAdminDetailDto> Handle(
            GetDoctorAdminDetailQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Tìm doctor
            var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy bác sĩ với Id {request.Id}.");

            // 2. Load Account
            var account = await _accountRepo.GetByIdAsync(doctor.AccountId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy tài khoản của bác sĩ.");

            // 3. Load Department
            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken);

            // 4. Map
            return DoctorMapper.ToAdminDetailDto(doctor, account, department);
        }
    }
}
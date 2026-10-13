using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Doctor;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    /// <summary>
    /// Doctor tự xem profile của mình.
    /// URL: GET /api/doctors/me
    /// 
    /// Tìm doctor theo AccountId (từ JWT).
    /// </summary>
    public record GetDoctorSelfProfileQuery(Guid AccountId)
        : IRequest<DoctorSelfProfileDto>;

    public class GetDoctorSelfProfileQueryHandler
        : IRequestHandler<GetDoctorSelfProfileQuery, DoctorSelfProfileDto>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetDoctorSelfProfileQueryHandler(
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<DoctorSelfProfileDto> Handle(
            GetDoctorSelfProfileQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Tìm doctor theo AccountId
            var doctor = await _doctorRepo.GetQueryable()
                .FirstOrDefaultAsync(d => d.AccountId == request.AccountId, cancellationToken)
                ?? throw new NotFoundException(
                    "Tài khoản này chưa được liên kết với hồ sơ bác sĩ.");

            // 2. Load Account
            var account = await _accountRepo.GetByIdAsync(doctor.AccountId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy tài khoản của bác sĩ.");

            // 3. Load Department
            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken);

            // 4. Map
            return DoctorMapper.ToSelfProfileDto(doctor, account, department);
        }
    }
}
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Doctor;
using BaseClinic.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    /// <summary>
    /// Chi tiết bác sĩ công khai theo SLUG.
    /// URL: GET /api/doctors/{slug}
    /// </summary>
    public record GetDoctorPublicProfileQuery(string Slug)
        : IRequest<DoctorPublicProfileDto>;

    public class GetDoctorPublicProfileQueryHandler
        : IRequestHandler<GetDoctorPublicProfileQuery, DoctorPublicProfileDto>
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public GetDoctorPublicProfileQueryHandler(
            IDoctorRepository doctorRepo,
            IAccountRepository accountRepo,
            IDepartmentRepository departmentRepo)
        {
            _doctorRepo = doctorRepo;
            _accountRepo = accountRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<DoctorPublicProfileDto> Handle(
            GetDoctorPublicProfileQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Tìm doctor theo slug
            var doctor = await _doctorRepo.GetQueryable()
                .FirstOrDefaultAsync(d => d.Slug == request.Slug, cancellationToken)
                ?? throw new NotFoundException(
                    $"Không tìm thấy bác sĩ với slug '{request.Slug}'.");

            // 2. Load Account (để có FullName)
            var account = await _accountRepo.GetByIdAsync(doctor.AccountId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy tài khoản của bác sĩ.");

            // 3. Load Department
            var department = await _departmentRepo.GetByIdAsync(
                doctor.DepartmentId, cancellationToken);

            // 4. Map
            return DoctorMapper.ToPublicProfileDto(doctor, account.FullName, department);
        }
    }
}
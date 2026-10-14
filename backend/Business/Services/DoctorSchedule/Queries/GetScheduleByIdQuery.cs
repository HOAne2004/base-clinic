
using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.DoctorSchedule.Queries
{
    /// <summary>
    /// Chi tiết 1 ca làm việc — có đầy đủ slots.
    /// URL: GET /api/doctor-schedules/{id}
    /// </summary>
    public record GetScheduleByIdQuery(Guid Id) : IRequest<DoctorScheduleDetailDto>;

    public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, DoctorScheduleDetailDto>
    {
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public GetScheduleByIdQueryHandler(IDoctorScheduleRepository doctorScheduleRepository, IAccountRepository accountRepository, IDepartmentRepository departmentRepository)
        {
            _doctorScheduleRepository = doctorScheduleRepository;
            _accountRepository = accountRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<DoctorScheduleDetailDto> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Load schedule + items + doctor
            var schedule = await _doctorScheduleRepository.GetByIdWithItemsAsync(
                request.Id, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy ca làm việc");

            // 2. Load Account
            var account = await _accountRepository.GetByIdAsync(
                schedule.Doctor.AccountId, cancellationToken);

            // 3. Load Department
            var department = await _departmentRepository.GetByIdAsync(
                schedule.Doctor.DepartmentId, cancellationToken);

            // 4. Map
            return DoctorScheduleMapper.ToDetailDto(
                schedule: schedule,
                doctor: schedule.Doctor,
                doctorFullName: account?.FullName ?? string.Empty,
                doctorCode: schedule.Doctor.DoctorCode,
                department: department);
        }
    }
}

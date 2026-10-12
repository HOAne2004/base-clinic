using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Patient;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Queries
{
    /// <summary>
    /// Lấy hồ sơ Patient primary của Account đang đăng nhập.
    /// Trả về null nếu Account chưa có Patient primary.
    /// </summary>
    public record GetPrimaryPatientQuery(Guid AccountId)
        : IRequest<PatientDto?>;

    public class GetPrimaryPatientQueryHandler
        : IRequestHandler<GetPrimaryPatientQuery, PatientDto?>
    {
        private readonly IPatientRepository _patientRepo;

        public GetPrimaryPatientQueryHandler(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public async Task<PatientDto?> Handle(
            GetPrimaryPatientQuery request,
            CancellationToken cancellationToken)
        {
            var patient = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken);

            return patient == null ? null : PatientMapper.ToDto(patient);
        }
    }
}
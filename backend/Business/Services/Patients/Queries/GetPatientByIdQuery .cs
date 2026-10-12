using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Mappers;
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Queries
{
    public record GetPatientByIdQuery(Guid Id) : IRequest<PatientDto>;

    public class GetPatientByIdQueryHandler
        : IRequestHandler<GetPatientByIdQuery, PatientDto>
    {
        private readonly IPatientRepository _patientRepo;

        public GetPatientByIdQueryHandler(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public async Task<PatientDto> Handle(
            GetPatientByIdQuery request,
            CancellationToken cancellationToken)
        {
            var p = await _patientRepo.GetByIdWithAccountAsync(request.Id, cancellationToken);

            if (p == null)
                throw new NotFoundException($"Không tìm thấy bệnh nhân với Id {request.Id}.");

            return PatientMapper.ToDto(p);
        }
    }
}
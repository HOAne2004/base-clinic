using BaseClinic.Business.Interfaces.Repositories;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Exceptions;
using MediatR;

namespace BaseClinic.Business.Services.Patients.Queries
{
    /// <summary>
    /// Lấy danh sách dependent của Patient primary hiện tại.
    /// </summary>
    public record GetMyDependentsQuery(Guid AccountId)
        : IRequest<List<DependentListItemDto>>;

    public class GetMyDependentsQueryHandler
        : IRequestHandler<GetMyDependentsQuery, List<DependentListItemDto>>
    {
        private readonly IPatientRepository _patientRepo;

        public GetMyDependentsQueryHandler(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public async Task<List<DependentListItemDto>> Handle(
            GetMyDependentsQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Tìm Patient primary của Account
            var primary = await _patientRepo.GetPrimaryByAccountIdAsync(
                request.AccountId, cancellationToken);

            if (primary == null)
                throw new DomainException(
                    "Tài khoản này chưa có hồ sơ bệnh nhân.");

            // 2. Lấy dependents
            var dependents = await _patientRepo.GetDependentsAsync(
                primary.Id, cancellationToken);

            // 3. Map
            return dependents.Select(d => new DependentListItemDto(
                Id: d.Id,
                PatientCode: d.PatientCode,
                FullName: d.FullName,
                Gender: d.Gender,
                DateOfBirth: d.DateOfBirth,
                IdentityNumber: d.IdentityNumber,
                HealthInsuranceNumber: d.HealthInsuranceNumber,
                Relationship: d.RelationshipType?.ToString(),
                CreatedDate: d.CreatedDate
            )).ToList();
        }
    }
}
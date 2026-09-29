using BaseClinic.DataAccess;
using BaseClinic.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Patients.Queries
{
    public record DependentDto(
        Guid PatientId,
        string PatientCode,
        string FullName,
        DateTime? DateOfBirth,
        bool? Gender,
        string RelationshipType
    );

    public record GetMyDependentsQuery(Guid AccountId) : IRequest<List<DependentDto>>;

    public class GetMyDependentsQueryHandler : IRequestHandler<GetMyDependentsQuery, List<DependentDto>>
    {
        private readonly ClinicDbContext _context;

        public GetMyDependentsQueryHandler(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<DependentDto>> Handle(GetMyDependentsQuery request, CancellationToken cancellationToken)
        {
            // Join bảng PatientDelegation và bảng Patient
            var query = from d in _context.PatientDelegations
                        join p in _context.Patients on d.TargetPatientId equals p.Id
                        where d.ObserverAccountId == request.AccountId
                              && d.Status == DelegationStatus.Accepted // Chỉ lấy những hồ sơ ĐÃ ĐƯỢC CẤP QUYỀN
                                                                       // In the LINQ select, ensure p.FullName is not null by providing a fallback value (e.g., empty string)
                        select new DependentDto(
                            p.Id,
                            p.PatientCode,
                            p.FullName ?? string.Empty,
                            p.DateOfBirth,
                            p.Gender,
                            d.RelationshipType.ToString()
                        );

            return await query.ToListAsync(cancellationToken);
        }
    }
}
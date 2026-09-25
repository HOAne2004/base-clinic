using BaseClinic.Business.Models;
using BaseClinic.Business.Services.Doctors.Queries;
using BaseClinic.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    public record GetDoctorDetailForAdminQuery(Guid Id) : IRequest<DoctorAdminDetailDto>;

}
public class GetDoctorDetailForAdminQueryHandler : IRequestHandler<GetDoctorDetailForAdminQuery, DoctorAdminDetailDto>
{
    private readonly ClinicDbContext _context;

    public GetDoctorDetailForAdminQueryHandler(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<DoctorAdminDetailDto> Handle(GetDoctorDetailForAdminQuery request, CancellationToken cancellationToken)
    {
        var query = from d in _context.Doctors
                    join a in _context.Accounts on d.AccountId equals a.Id
                    join dep in _context.Departments on d.DepartmentId equals dep.Id
                    where d.Id == request.Id
                    select new DoctorAdminDetailDto(
                        d.Id,
                        d.AccountId,
                        d.DoctorCode,
                        a.FullName,
                        a.PhoneNumber,
                        a.Email,
                        dep.Name,
                        a.Status.ToString(),
                        a.LastLoginAt,
                        d.Avatar,
                        d.FullAddress,
                        d.Gender,
                        d.DateOfBirth,
                        d.IdentityNumber,
                        d.LicenseNumber,
                        d.ConsultationFee,
                        d.Description
                    );

        var doctorDetail = await query.FirstOrDefaultAsync(cancellationToken);

        if (doctorDetail == null)
        {
            throw new InvalidOperationException("Không tìm thấy thông tin bác sĩ này.");
        }

        return doctorDetail;
    }
}


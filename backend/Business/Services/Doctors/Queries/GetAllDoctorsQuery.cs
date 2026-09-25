using BaseClinic.Business.Models;
using BaseClinic.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.Business.Services.Doctors.Queries
{
    public record GetAllDoctorsQuery() : IRequest<List<DoctorListDto>>;

    public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, List<DoctorListDto>>
    {
        private readonly ClinicDbContext _context;

        public GetAllDoctorsQueryHandler(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorListDto>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
        {
            // Thực hiện Join 3 bảng: Doctors, Accounts, Departments
            var query = from d in _context.Doctors
                        join a in _context.Accounts on d.AccountId equals a.Id
                        join dep in _context.Departments on d.DepartmentId equals dep.Id
                        orderby d.CreatedAt descending // Sắp xếp bác sĩ mới tạo lên đầu
                        select new DoctorListDto(
                            d.Id,
                            d.AccountId,
                            d.DoctorCode,
                            a.FullName,
                            a.PhoneNumber,
                            a.Email,
                            dep.Name,
                            a.Status.ToString() // Lấy tên trạng thái từ enum AccountStatus
                        );

            // Thực thi truy vấn và trả về danh sách
            return await query.ToListAsync(cancellationToken);
        }
    }
}
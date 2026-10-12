using BaseClinic.Business.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Services
{
    public class AppointmentCodeGenerator : IAppointmentCodeGenerator
    {
        private readonly ClinicDbContext _context;
        public AppointmentCodeGenerator(ClinicDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
        {
            // Định dạng tiền tố: LH + NămThángNgày (VD: LH20260923)
            var today = DateTime.UtcNow.Date;
            var prefix = $"LH{today:yyyyMMdd}";

            // Đếm xem trong ngày hôm nay đã có bao nhiêu mã bắt đầu bằng tiền tố này
            var count = await _context.Patients
                .Where(p => p.PatientCode.StartsWith(prefix))
                .CountAsync(cancellationToken);

            // Tăng số đếm lên 1 và đệm thêm các số 0 ở trước cho đủ 4 chữ số (VD: 0001)
            var sequence = (count + 1).ToString("D4");

            // Kết quả: LH20260923-0001
            return $"{prefix}-{sequence}";
        }
    }
}

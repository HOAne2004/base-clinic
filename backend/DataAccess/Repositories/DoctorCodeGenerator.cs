using BaseClinic.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading;

namespace BaseClinic.DataAccess.Repositories
{
    public class DoctorCodeGenerator : IDoctorCodeGenerator
    {
        private readonly ClinicDbContext _context;
        public DoctorCodeGenerator(ClinicDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateAsync(CancellationToken cancellation = default)
        {
            const string prefix = "BS";

            // Lấy bác sĩ được tạo cuối cùng dựa trên Id hoặc ngày tạo để đảm bảo thứ tự
            var lastDoctor = await _context.Doctors
                .OrderByDescending(d => d.Id) // Hoặc OrderByDescending(d => d.CreatedAt) nếu bạn dùng AuditableEntity
                .FirstOrDefaultAsync(cancellation);

            if (lastDoctor == null || string.IsNullOrWhiteSpace(lastDoctor.DoctorCode))
            {
                // Nếu chưa có bác sĩ nào trong hệ thống, trả về mã đầu tiên
                return $"{prefix}00001";
            }

            // Dùng Regex để tách riêng phần chữ và phần số của mã cũ (VD: "BS00015" -> "00015")
            var match = Regex.Match(lastDoctor.DoctorCode, @"\d+");

            if (match.Success && int.TryParse(match.Value, out int currentNumber))
            {
                int nextNumber = currentNumber + 1;
                // Format lại thành 5 chữ số có số 0 ở đầu (00016)
                return $"{prefix}{nextNumber:D5}";
            }

            // Fallback (Phòng hờ trường hợp mã cũ bị sai định dạng, tự động sinh mã ngẫu nhiên)
            var randomSuffix = new Random().Next(10000, 99999);
            return $"{prefix}{randomSuffix}";
        }
    }
}

using BaseClinic.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Repositories
{
    public class PatientCodeGenerator : IPatientCodeGenerator
    {
        private readonly ClinicDbContext _context;
        public PatientCodeGenerator (ClinicDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
        {
            // Định dạng tiền tố: BN + NămThángNgày (VD: BN20260923)
            var today = DateTime.UtcNow.Date;
            var prefix = $"BN{today:yyyyMMdd}";

            // Đếm xem trong ngày hôm nay đã có bao nhiêu mã bắt đầu bằng tiền tố này
            var count = await _context.Patients
                .Where(p => p.PatientCode.StartsWith(prefix))
                .CountAsync(cancellationToken);

            // Tăng số đếm lên 1 và đệm thêm các số 0 ở trước cho đủ 4 chữ số (VD: 0001)
            var sequence = (count + 1).ToString("D4");

            // Kết quả: BN20260923-0001
            return $"{prefix}-{sequence}";
        }
    }
}

using BaseClinic.Business.Interfaces.Services;
using BaseClinic.Domain.Constants;
using BaseClinic.Domain.Entities;
using BaseClinic.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Seeding
{
    /// <summary>
    /// Seed dữ liệu mẫu cho Doctors + Accounts + AccountRoles.
    /// 
    /// 2 PASS:
    /// - Pass 1: Tạo Accounts + gán Role → SaveChanges → có Account.Id.
    /// - Pass 2: Tạo Doctors với AccountId thực → SaveChanges.
    /// 
    /// PASSWORD mặc định: "Doctor@123".
    /// </summary>
    public static class DoctorSeeder
    {
        private const string DefaultPassword = "Doctor@123";

        private record DoctorSeed(
            string DepartmentCode,
            string FullName,
            string PhoneNumber,
            string? Email,
            string Title,
            string Specialization,
            int YearsOfExperience,
            string? Biography,
            string Languages,
            decimal ConsultationFee,
            bool Gender,
            int BirthYear,
            int BirthMonth,
            int BirthDay,
            string? FullAddress,
            string? LicenseNumber,
            int SortOrder);

        // ==================== SEED DATA ====================
        private static readonly DoctorSeed[] SeedData =
        {
            new("CAP_CUU", "Nguyễn Văn An", "0901000001", "an.nguyen@clinic.vn",
                "BS", "Cấp cứu - Hồi sức", 12,
                "Bác sĩ chuyên khoa Cấp cứu với 12 năm kinh nghiệm, từng công tác tại Bệnh viện Chợ Rẫy.",
                "vi,en", 500000, true, 1982, 5, 15,
                "123 Lê Lợi, Phường 1, Hà Nội", "CC-HN-0001234", 1),

            new("CAP_CUU", "Trần Thị Bích", "0901000002", "bich.tran@clinic.vn",
                "ThS", "Cấp cứu - Nhi", 8,
                "Thạc sĩ y khoa, chuyên về cấp cứu nhi khoa và sơ sinh.",
                "vi,en", 600000, false, 1986, 8, 22,
                "456 Nguyễn Huệ, Phường 2, TP.HCM", "CC-HN-0002345", 2),

            new("NOI_TONG_HOP", "Lê Minh Châu", "0901000003", "chau.le@clinic.vn",
                "TS", "Nội tim mạch", 15,
                "Tiến sĩ y khoa, chuyên gia về tim mạch can thiệp và siêu âm tim.",
                "vi,en,fr", 800000, true, 1979, 3, 10,
                "789 Trần Hưng Đạo, Phường 3, Hà Nội", "CC-HN-0003456", 3),

            new("NOI_TONG_HOP", "Phạm Thu Dung", "0901000004", "dung.pham@clinic.vn",
                "PGS", "Nội tiêu hóa", 20,
                "Phó giáo sư, chuyên gia đầu ngành về nội soi tiêu hóa và bệnh lý gan mật.",
                "vi,en", 1000000, false, 1974, 11, 5,
                "321 Lý Thường Kiệt, Phường 4, TP.HCM", "CC-HN-0004567", 4),

            new("NOI_TONG_HOP", "Hoàng Văn Em", "0901000005", "em.hoang@clinic.vn",
                "BS", "Nội thần kinh", 7,
                "Bác sĩ chuyên khoa Nội thần kinh, có kinh nghiệm điều trị đau đầu, động kinh.",
                "vi,en", 500000, true, 1987, 6, 18,
                "654 Hai Bà Trưng, Phường 5, Hà Nội", "CC-HN-0005678", 5),

            new("NGOAI_TONG_HOP", "Vũ Đức Giang", "0901000006", "giang.vu@clinic.vn",
                "TS", "Ngoại tiêu hóa", 18,
                "Tiến sĩ y khoa, chuyên gia phẫu thuật nội soi ổ bụng và tiêu hóa.",
                "vi,en", 1200000, true, 1976, 9, 12,
                "987 Điện Biên Phủ, Phường 6, TP.HCM", "CC-HN-0006789", 6),

            new("NGOAI_TONG_HOP", "Đặng Thị Hoa", "0901000007", "hoa.dang@clinic.vn",
                "BS", "Ngoại chấn thương chỉnh hình", 10,
                "Bác sĩ chuyên khoa chấn thương chỉnh hình, có kinh nghiệm phẫu thuật khớp.",
                "vi,en", 700000, false, 1984, 12, 25,
                "147 Võ Văn Tần, Phường 7, Hà Nội", "CC-HN-0007890", 7),

            new("NHI", "Bùi Thị Kim", "0901000008", "kim.bui@clinic.vn",
                "PGS", "Nhi tổng hợp", 22,
                "Phó giáo sư Nhi khoa, chuyên gia về dinh dưỡng và phát triển trẻ em.",
                "vi,en", 900000, false, 1972, 4, 8,
                "258 Cách Mạng Tháng 8, Phường 8, TP.HCM", "CC-HN-0008901", 8),

            new("NHI", "Ngô Văn Long", "0901000009", "long.ngo@clinic.vn",
                "TS", "Nhi hô hấp", 14,
                "Tiến sĩ y khoa, chuyên về hen phế quản và bệnh lý hô hấp trẻ em.",
                "vi,en", 800000, true, 1980, 7, 14,
                "369 Nguyễn Trãi, Phường 9, Hà Nội", "CC-HN-0009012", 9),

            new("NHI", "Dương Thị Mai", "0901000010", "mai.duong@clinic.vn",
                "ThS", "Nhi sơ sinh", 9,
                "Thạc sĩ y khoa, chuyên gia chăm sóc và điều trị trẻ sơ sinh non tháng.",
                "vi,en", 700000, false, 1985, 2, 20,
                "741 Lê Văn Việt, Phường 10, TP.HCM", "CC-HN-0010123", 10),

            new("SAN", "Lý Thị Ngọc", "0901000011", "ngoc.ly@clinic.vn",
                "TS", "Sản phụ khoa", 16,
                "Tiến sĩ y khoa, chuyên gia về thai kỳ nguy cơ cao và vô sinh hiếm muộn.",
                "vi,en", 1100000, false, 1978, 10, 3,
                "852 Pasteur, Phường 11, Hà Nội", "CC-HN-0011234", 11),

            new("SAN", "Trịnh Văn Phúc", "0901000012", "phuc.trinh@clinic.vn",
                "BS", "Sản - Nam khoa", 8,
                "Bác sĩ chuyên khoa Sản - Nam khoa, có kinh nghiệm tư vấn sinh sản.",
                "vi,en", 600000, true, 1986, 1, 28,
                "963 Lê Thánh Tôn, Phường 12, TP.HCM", "CC-HN-0012345", 12),

            new("MAT", "Hồ Thị Quỳnh", "0901000013", "quynh.ho@clinic.vn",
                "ThS", "Nhãn khoa", 11,
                "Thạc sĩ y khoa, chuyên về phẫu thuật đục thủy tinh thể và Lasik.",
                "vi,en", 750000, false, 1983, 5, 16,
                "159 Nam Kỳ Khởi Nghĩa, Phường 13, Hà Nội", "CC-HN-0013456", 13),

            new("TAI_MUI_HONG", "Đinh Văn Sơn", "0901000014", "son.dinh@clinic.vn",
                "TS", "Tai Mũi Họng", 17,
                "Tiến sĩ y khoa, chuyên về phẫu thuật nội soi xoang và thanh quản.",
                "vi,en", 850000, true, 1977, 8, 30,
                "357 Nguyễn Thị Minh Khai, Phường 14, TP.HCM", "CC-HN-0014567", 14),

            new("RANG_HAM_MAT", "Lưu Thị Tâm", "0901000015", "tam.luu@clinic.vn",
                "ThS", "Răng Hàm Mặt", 13,
                "Thạc sĩ y khoa, chuyên về chỉnh nha và cấy ghép implant.",
                "vi,en", 900000, false, 1981, 6, 7,
                "486 Trường Chinh, Phường 15, Hà Nội", "CC-HN-0015678", 15),

            new("DA_LIEU", "Phan Văn Uy", "0901000016", "uy.phan@clinic.vn",
                "BS", "Da liễu thẩm mỹ", 6,
                "Bác sĩ chuyên khoa Da liễu, có kinh nghiệm điều trị mụn và trẻ hóa da.",
                "vi,en", 550000, true, 1988, 9, 22,
                "753 Hoàng Hoa Thám, Phường 16, TP.HCM", "CC-HN-0016789", 16),

            new("NOI_TONG_HOP", "Mai Thị Vân", "0901000017", "van.mai@clinic.vn",
                "BS", "Nội tiết", 5,
                "Bác sĩ chuyên khoa Nội tiết, chuyên về đái tháo đường và tuyến giáp.",
                "vi,en", 500000, false, 1989, 11, 11,
                "864 Lạc Long Quân, Phường 17, Hà Nội", "CC-HN-0017890", 17),

            new("NHI", "Tạ Văn Xuân", "0901000018", "xuan.ta@clinic.vn",
                "BS", "Nhi tiêu hóa", 4,
                "Bác sĩ Nhi khoa, có kinh nghiệm điều trị rối loạn tiêu hóa ở trẻ.",
                "vi,en", 500000, true, 1990, 3, 15,
                "975 Âu Cơ, Phường 18, TP.HCM", "CC-HN-0018901", 18),
        };

        public static async Task SeedAsync(
            ClinicDbContext context,
            IPasswordHasher passwordHasher,
            CancellationToken cancellationToken = default)
        {
            // ==================== BƯỚC 1: Load dictionaries ====================
            var departments = await context.Departments
                .ToDictionaryAsync(d => d.DepartmentCode, d => d, cancellationToken);

            var doctorRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Code == SystemRoles.Doctor, cancellationToken)
                ?? throw new InvalidOperationException(
                    "Role 'Doctor' chưa được seed. Vui lòng chạy RoleSeeder trước.");

            // Existing phones
            var existingPhones = await context.Accounts
                .Select(a => a.PhoneNumber)
                .ToListAsync(cancellationToken);
            var existingPhoneSet = existingPhones.ToHashSet();

            // Existing slugs
            var existingSlugs = await context.Doctors
                .Select(d => d.Slug)
                .ToListAsync(cancellationToken);
            var slugSet = existingSlugs.ToHashSet();

            // ==================== BƯỚC 2: PASS 1 — Tạo Accounts ====================
            // Map: PhoneNumber → Account (để dùng ở Pass 2)
            var accountMap = new Dictionary<string, Account>();
            var accountsToAdd = new List<Account>();

            foreach (var seed in SeedData)
            {
                if (existingPhoneSet.Contains(seed.PhoneNumber))
                    continue;

                var hashedPassword = passwordHasher.Hash(DefaultPassword);

                var account = new Account(
                    fullName: seed.FullName,
                    phoneNumber: seed.PhoneNumber,
                    passwordHash: hashedPassword,
                    email: seed.Email);

                account.VerifyPhoneNumber();
                account.Activate();
                account.AssignRole(doctorRole.Id);

                accountsToAdd.Add(account);
                accountMap[seed.PhoneNumber] = account;
            }

            if (accountsToAdd.Any())
            {
                context.Accounts.AddRange(accountsToAdd);
                await context.SaveChangesAsync(cancellationToken);

                // Sau SaveChanges, mỗi Account đã có Id
            }

            // ==================== BƯỚC 3: PASS 2 — Tạo Doctors ====================
            var doctorsToAdd = new List<Doctor>();

            foreach (var seed in SeedData)
            {
                // Skip nếu doctor đã tồn tại (idempotent — check theo slug/doctorCode)
                // Nhưng vì slug dựa vào FullName, có thể check qua phone → account → doctor
                // Đơn giản: kiểm tra Doctor có AccountId trùng
                if (!accountMap.TryGetValue(seed.PhoneNumber, out var account))
                {
                    // Account đã tồn tại từ trước (không phải seed session này)
                    // Tìm account theo phone
                    account = await context.Accounts
                        .FirstOrDefaultAsync(a => a.PhoneNumber == seed.PhoneNumber, cancellationToken);

                    if (account == null)
                        continue;

                    // Check doctor đã tồn tại chưa
                    var doctorExists = await context.Doctors
                        .AnyAsync(d => d.AccountId == account.Id, cancellationToken);

                    if (doctorExists)
                        continue;
                }

                if (!departments.TryGetValue(seed.DepartmentCode, out var department))
                    continue;

                // Sinh slug unique
                var slug = GenerateUniqueSlug(seed.FullName, slugSet);
                slugSet.Add(slug);

                // ⚡ AccountId giờ đã có giá trị thực
                var doctor = new Doctor(
                    accountId: account.Id,
                    departmentId: department.Id,
                    doctorCode: $"BS{seed.SortOrder:D4}",
                    licenseNumber: seed.LicenseNumber,
                    licenseIssueDate: DateTime.UtcNow.AddYears(-5),
                    licenseExpiryDate: DateTime.UtcNow.AddYears(5),
                    licenseIssuedBy: "Sở Y tế",
                    sortOrder: seed.SortOrder,
                    slug: slug);

                doctor.UpdateProfile(
                    avatar: null,
                    fullAddress: seed.FullAddress,
                    gender: seed.Gender,
                    dateOfBirth: new DateTime(seed.BirthYear, seed.BirthMonth, seed.BirthDay),
                    identityNumber: null,
                    description: null);

                doctor.UpdateProfessionalInfo(
                    title: seed.Title,
                    specialization: seed.Specialization,
                    yearsOfExperience: seed.YearsOfExperience,
                    biography: seed.Biography,
                    languages: seed.Languages);

                doctor.UpdateConsultationFee(seed.ConsultationFee);

                doctorsToAdd.Add(doctor);
            }

            if (doctorsToAdd.Any())
            {
                context.Doctors.AddRange(doctorsToAdd);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        // ==================== HELPERS ====================

        private static string GenerateUniqueSlug(string fullName, HashSet<string> existingSlugs)
        {
            var baseSlug = SlugHelper.Generate(fullName);

            if (string.IsNullOrEmpty(baseSlug))
                baseSlug = "bac-si";

            var candidate = baseSlug;
            int suffix = 2;

            while (existingSlugs.Contains(candidate))
            {
                candidate = $"{baseSlug}-{suffix}";
                suffix++;
            }

            return candidate;
        }
    }
}
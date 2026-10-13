using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseClinic.DataAccess.Seeding
{
    /// <summary>
    /// Seed dữ liệu mẫu cho Departments.
    /// Idempotent: chạy nhiều lần không tạo trùng (check theo DepartmentCode).
    /// </summary>
    public static class DepartmentSeeder
    {
        /// <summary>
        /// Danh sách khoa mẫu — bao gồm cả:
        /// - Khoa 24/24 (Cấp cứu).
        /// - Khoa giờ hành chính.
        /// - Khoa tạm ngưng (IsActive = false).
        /// </summary>
        private record DepartmentSeed(
            string Code,
            string Name,
            string? ImageUrl,
            string? ShortDescription,
            string? FullDescription,
            string? Location,
            string? PhoneNumber,
            bool Is24Hours,
            TimeOnly? OpeningTime,
            TimeOnly? ClosingTime,
            int SortOrder,
            bool IsActive);

        private static readonly DepartmentSeed[] SeedData =
        {
            // ==================== KHOAS 24/24 ====================
            new(
                Code: "CAP_CUU",
                Name: "Khoa Cấp cứu",
                ImageUrl: null,
                ShortDescription: "Tiếp nhận và xử lý các ca cấp cứu 24/24.",
                FullDescription: "Khoa Cấp cứu hoạt động 24/24, tiếp nhận mọi trường hợp " +
                                 "cấp cứu, tai nạn, ngộ độc, đột quỵ... Đội ngũ bác sĩ " +
                                 "chuyên khoa giàu kinh nghiệm, trang thiết bị hiện đại.",
                Location: "Tầng 1, Tòa A",
                PhoneNumber: "19001001",
                Is24Hours: true,
                OpeningTime: null,
                ClosingTime: null,
                SortOrder: 1,
                IsActive: true),

            // ==================== KHOA GIỜ HÀNH CHÍNH ====================
            new(
                Code: "NOI_TONG_HOP",
                Name: "Khoa Nội tổng hợp",
                ImageUrl: null,
                ShortDescription: "Khám và điều trị các bệnh nội khoa.",
                FullDescription: "Khoa Nội tổng hợp chuyên khám, chẩn đoán và điều trị " +
                                 "các bệnh lý nội khoa: tim mạch, hô hấp, tiêu hóa, " +
                                 "nội tiết, thần kinh...",
                Location: "Tầng 2, Tòa A",
                PhoneNumber: "19001002",
                Is24Hours: false,
                OpeningTime: new TimeOnly(7, 0),
                ClosingTime: new TimeOnly(17, 0),
                SortOrder: 2,
                IsActive: true),

            new(
                Code: "NGOAI_TONG_HOP",
                Name: "Khoa Ngoại tổng hợp",
                ImageUrl: null,
                ShortDescription: "Khám và phẫu thuật các bệnh ngoại khoa.",
                FullDescription: "Khoa Ngoại tổng hợp thực hiện các ca phẫu thuật " +
                                 "từ đơn giản đến phức tạp: tiêu hóa, tiết niệu, " +
                                 "chấn thương chỉnh hình...",
                Location: "Tầng 3, Tòa A",
                PhoneNumber: "19001003",
                Is24Hours: false,
                OpeningTime: new TimeOnly(7, 30),
                ClosingTime: new TimeOnly(16, 30),
                SortOrder: 3,
                IsActive: true),

            new(
                Code: "NHI",
                Name: "Khoa Nhi",
                ImageUrl: null,
                ShortDescription: "Khám và điều trị bệnh cho trẻ em.",
                FullDescription: "Khoa Nhi chuyên khám, điều trị và chăm sóc sức khỏe " +
                                 "cho trẻ em từ sơ sinh đến 16 tuổi. Bao gồm khám " +
                                 "định kỳ, tiêm chủng, điều trị bệnh lý nhi khoa.",
                Location: "Tầng 2, Tòa B",
                PhoneNumber: "19001004",
                Is24Hours: false,
                OpeningTime: new TimeOnly(7, 0),
                ClosingTime: new TimeOnly(17, 0),
                SortOrder: 4,
                IsActive: true),

            new(
                Code: "SAN",
                Name: "Khoa Sản",
                ImageUrl: null,
                ShortDescription: "Khám thai và chăm sóc sức khỏe sinh sản.",
                FullDescription: "Khoa Sản cung cấp dịch vụ khám thai định kỳ, " +
                                 "siêu âm thai, tư vấn sức khỏe sinh sản, " +
                                 "đỡ sinh và chăm sóc sau sinh.",
                Location: "Tầng 3, Tòa B",
                PhoneNumber: "19001005",
                Is24Hours: false,
                OpeningTime: new TimeOnly(7, 0),
                ClosingTime: new TimeOnly(17, 0),
                SortOrder: 5,
                IsActive: true),

            new(
                Code: "MAT",
                Name: "Khoa Mắt",
                ImageUrl: null,
                ShortDescription: "Khám và điều trị các bệnh về mắt.",
                FullDescription: "Khoa Mắt thực hiện khám, đo thị lực, chẩn đoán và " +
                                 "điều trị các bệnh lý về mắt: cận thị, viễn thị, " +
                                 "đục thủy tinh thể, glocom...",
                Location: "Tầng 4, Tòa A",
                PhoneNumber: "19001006",
                Is24Hours: false,
                OpeningTime: new TimeOnly(8, 0),
                ClosingTime: new TimeOnly(16, 0),
                SortOrder: 6,
                IsActive: true),

            new(
                Code: "TAI_MUI_HONG",
                Name: "Khoa Tai Mũi Họng",
                ImageUrl: null,
                ShortDescription: "Khám và điều trị bệnh tai mũi họng.",
                FullDescription: "Khoa Tai Mũi Họng chuyên khám, nội soi và điều trị " +
                                 "các bệnh lý về tai, mũi, họng, thanh quản.",
                Location: "Tầng 4, Tòa A",
                PhoneNumber: "19001007",
                Is24Hours: false,
                OpeningTime: new TimeOnly(8, 0),
                ClosingTime: new TimeOnly(16, 0),
                SortOrder: 7,
                IsActive: true),

            new(
                Code: "RANG_HAM_MAT",
                Name: "Khoa Răng Hàm Mặt",
                ImageUrl: null,
                ShortDescription: "Khám và điều trị các bệnh về răng miệng.",
                FullDescription: "Khoa Răng Hàm Mặt cung cấp dịch vụ khám, điều trị " +
                                 "và chăm sóc răng miệng toàn diện: cạo vôi, trám răng, " +
                                 "nhổ răng, niềng răng, implant...",
                Location: "Tầng 5, Tòa A",
                PhoneNumber: "19001008",
                Is24Hours: false,
                OpeningTime: new TimeOnly(8, 0),
                ClosingTime: new TimeOnly(17, 0),
                SortOrder: 8,
                IsActive: true),

            new(
                Code: "DA_LIEU",
                Name: "Khoa Da liễu",
                ImageUrl: null,
                ShortDescription: "Khám và điều trị các bệnh về da.",
                FullDescription: "Khoa Da liễu chuyên khám và điều trị các bệnh lý " +
                                 "về da: mụn, viêm da, dị ứng, vảy nến, nấm da...",
                Location: "Tầng 5, Tòa A",
                PhoneNumber: "19001009",
                Is24Hours: false,
                OpeningTime: new TimeOnly(8, 0),
                ClosingTime: new TimeOnly(16, 30),
                SortOrder: 9,
                IsActive: true),

            // ==================== KHOA TẠM NGƯNG (test UI) ====================
            new(
                Code: "PHONG_PHAU_THUAT",
                Name: "Phòng Phẫu thuật",
                ImageUrl: null,
                ShortDescription: "Phòng phẫu thuật chuyên biệt.",
                FullDescription: "Phòng Phẫu thuật đang trong quá trình nâng cấp " +
                                 "trang thiết bị, tạm ngưng phục vụ để bảo trì.",
                Location: "Tầng 6, Tòa A",
                PhoneNumber: "19001010",
                Is24Hours: false,
                OpeningTime: new TimeOnly(7, 0),
                ClosingTime: new TimeOnly(17, 0),
                SortOrder: 10,
                IsActive: false),   // ← Tạm ngưng (test UI)
        };

        public static async Task SeedAsync(
            ClinicDbContext context,
            CancellationToken cancellationToken = default)
        {
            // 1. Lấy các Code đã có trong DB
            var existingCodes = await context.Departments
                .Select(d => d.DepartmentCode)
                .ToListAsync(cancellationToken);
            var existingSet = existingCodes.ToHashSet();

            // 2. Tạo mới những khoa chưa có
            var toAdd = new List<Department>();

            foreach (var seed in SeedData)
            {
                if (existingSet.Contains(seed.Code))
                    continue;

                var department = new Department(
                    name: seed.Name,
                    departmentCode: seed.Code,
                    imageUrl: seed.ImageUrl,
                    shortDescription: seed.ShortDescription,
                    fullDescription: seed.FullDescription,
                    location: seed.Location,
                    phoneNumber: seed.PhoneNumber,
                    is24Hours: seed.Is24Hours,
                    openingTime: seed.OpeningTime,
                    closingTime: seed.ClosingTime,
                    sortOrder: seed.SortOrder);

                // Set IsActive nếu cần (constructor mặc định = true)
                if (!seed.IsActive)
                    department.Deactivate();

                toAdd.Add(department);
            }

            if (toAdd.Any())
            {
                context.Departments.AddRange(toAdd);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
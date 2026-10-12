namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Request search bệnh nhân.
    /// 
    /// LOGIC KẾT HỢP:
    /// - Keyword → OR across multiple fields (Code, Name, CCCD, BHYT, Phone).
    /// - Các filter khác (Gender, IsPrimary, FromDate, ToDate) → AND.
    /// 
    /// VÍ DỤ:
    /// ?keyword=nguyen&gender=true&fromDate=2026-01-01
    /// → WHERE (Code LIKE %nguyen% OR Name LIKE %nguyen% OR ...)
    ///         AND Gender = true
    ///         AND CreatedDate >= 2026-01-01
    /// </summary>
    public class PatientSearchDto
    {
        /// <summary>Tìm theo mã BN, tên, CCCD, BHYT, SĐT (logic OR).</summary>
        public string? Keyword { get; set; }

        public bool? Gender { get; set; }

        public bool? IsPrimary { get; set; }

        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
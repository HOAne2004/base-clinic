namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// Filter cho danh sách bác sĩ.
    /// 
    /// SORT: SortOrder ASC → FullName ASC.
    /// </summary>
    public class DoctorSearchDto
    {
        /// <summary>Tìm theo FullName (Account), DoctorCode, hoặc Slug.</summary>
        public string? Keyword { get; set; }

        public Guid? DepartmentId { get; set; }

        public string? Specialization { get; set; }

        /// <summary>Lọc theo giới tính (true = Nam, false = Nữ).</summary>
        public bool? Gender { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsAvailableForOnline { get; set; }

        /// <summary>Phí khám tối thiểu.</summary>
        public decimal? MinFee { get; set; }

        /// <summary>Phí khám tối đa.</summary>
        public decimal? MaxFee { get; set; }

        /// <summary>Chỉ dùng cho Admin — lấy cả bác sĩ đã xóa mềm.</summary>
        public bool IncludeDeleted { get; set; } = false;

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
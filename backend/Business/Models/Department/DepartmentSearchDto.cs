namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// Filter cho danh sách khoa.
    /// 
    /// SORT: Luôn luôn theo SortOrder tăng dần (1 → 999).
    /// 
    /// USE CASE:
    /// - Admin: có thể truyền IncludeDeleted = true để xem cả khoa đã xóa mềm.
    /// - Guest/Patient: chỉ query với IsActive = true.
    /// </summary>
    public class DepartmentSearchDto
    {
        /// <summary>
        /// Tìm theo Name hoặc DepartmentCode (contains, không phân biệt hoa thường).
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>Lọc theo trạng thái hoạt động. Null = tất cả.</summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Chỉ dùng cho Admin — có lấy cả khoa đã xóa mềm không.
        /// Yêu cầu permission đặc biệt mới được set true.
        /// </summary>
        public bool IncludeDeleted { get; set; } = false;

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
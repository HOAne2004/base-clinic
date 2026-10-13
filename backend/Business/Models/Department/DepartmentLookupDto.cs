namespace BaseClinic.Business.Models.Department
{
    /// <summary>
    /// DTO rút gọn cho dropdown/select.
    /// Bao gồm SortOrder để FE sắp xếp đúng thứ tự.
    /// </summary>
    public record DepartmentLookupDto(
        Guid Id,
        string Name,
        string DepartmentCode,
        int SortOrder
    );
}
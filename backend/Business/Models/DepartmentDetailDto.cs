namespace BaseClinic.Business.Models
{
    public record DepartmentDetailDto(
        Guid Id,
        string Name, 
        string DepartmentCode,
        string? ImageUrl, 
        string? Description,
        bool IsActive);

}

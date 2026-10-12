namespace BaseClinic.Business.Models.Department
{
    public record DepartmentDetailDto(
        Guid Id,
        string Name, 
        string DepartmentCode,
        string? ImageUrl, 
        string? ShortDescription,
        string? FullDescription,
        bool IsActive);

}

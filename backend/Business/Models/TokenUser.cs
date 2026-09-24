namespace BaseClinic.Business.Models
{
    public sealed record TokenUser
        (Guid Id,
        string FullName,
        string Email,
        Guid SecurityStamp,
        Guid SessionId,
        List<string> Roles,
         List<string> Permissions
        );
    
}

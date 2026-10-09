namespace BaseClinic.Business.Models.Auth
{
    public sealed record TokenUser
        (Guid Id,
        string FullName,
        string PhoneNumber,
        Guid SecurityStamp,
        Guid SessionId,
        List<string> Roles
        );
    
}

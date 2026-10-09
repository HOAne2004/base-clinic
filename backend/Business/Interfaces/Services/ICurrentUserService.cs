namespace BaseClinic.Business.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid? AccountId { get; }
        string? UserName { get; } // map với CreatedBy
        bool IsInRole(string roleName);
    }
}

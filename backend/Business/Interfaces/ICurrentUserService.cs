namespace BaseClinic.Business.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? AccountId { get; }
        string? UserName { get; } // map với CreatedBy
        bool IsInRole(string roleName);
    }
}

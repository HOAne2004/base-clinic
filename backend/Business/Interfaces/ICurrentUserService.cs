namespace BaseClinic.Business.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? AccountId { get; }
        bool IsInRole(string roleName);
    }
}

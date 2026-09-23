
namespace BaseClinic.Domain.Constants
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionInfoAttribute : Attribute
    {
        public string Description { get; set; } = string.Empty;
    }
}
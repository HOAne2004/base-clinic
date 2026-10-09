
namespace BaseClinic.Domain.Constants
{
    /// <summary>
    /// Metadata cho mỗi Permission constant.
    /// Seeder sẽ đọc attribute này để tạo Permission entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionInfoAttribute : Attribute
    {
        public string Description { get; }
        public string Resource { get; }
        public string Action { get; }
        public int DisplayOrder { get; }

        public PermissionInfoAttribute(string description, string resource, string action, int displayOrder = 0)
        {
            Description = description;
            Resource = resource;
            Action = action;
            DisplayOrder = displayOrder;
        }
    }
}
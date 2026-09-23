using BaseClinic.Domain.Common;
using System;

namespace BaseClinic.Domain.Entities
{
    public class Permission : AuditableEntity
    {
        public string Name { get; private set; } = null!;
        public string Resource { get; private set; } = null!; 
        public string? Description { get; private set; }

        private Permission() { }

        public Permission(string name, string resource, string? description)
        {
            Name = name;
            Resource = resource;
            Description = description;
        }

        public void Update(string name, string resource, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên Permission không được để trống.", nameof(name));
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource không được để trống.", nameof(resource));

            Name = name.Trim();
            Resource = resource.Trim();
            Description = description?.Trim();
        }
    }
}
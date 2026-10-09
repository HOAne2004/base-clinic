using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);  // "patient:read" — không cần Unicode

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Resource)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // Unique Code
            builder.HasIndex(p => p.Code)
                .IsUnique();

            // Index cho nhóm theo Resource
            builder.HasIndex(p => p.Resource);

            // ==================== RELATIONSHIPS ====================
            // RolePermission đã cấu hình ở RolePermissionConfiguration
        }
    }
}
using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(r => r.Priority)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(r => r.IsSystemRole)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Unique Code
            builder.HasIndex(r => r.Code)
                .IsUnique();

            // ==================== RELATIONSHIPS ====================

            // 1. Role → RolePermission (1-N)
            builder.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Role.RolePermissions))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 2. Role → AccountRole (1-N)
            builder.HasMany(x => x.AccountRoles)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Role.AccountRoles))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
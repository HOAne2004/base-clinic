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

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(r => r.Description)
                .HasMaxLength(200);

            builder.Property(x => x.IsSystemRole)
                .IsRequired()
                .HasDefaultValue(false);

            // Mapping Navigation property (Backing field)
            builder.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Role.RolePermissions))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

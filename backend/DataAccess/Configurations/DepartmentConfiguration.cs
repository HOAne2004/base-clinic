using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(x => x.Id);

            // ==================== BASIC INFO ====================
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DepartmentCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.DepartmentCode).IsUnique();

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(Department.MaxSlugLength)
                .IsUnicode(false);   // Slug chỉ chứa a-z, 0-9, '-' → ASCII

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.ShortDescription)
                .HasMaxLength(200);

            builder.Property(x => x.FullDescription)
                .HasMaxLength(1000);

            // ==================== LOCATION & CONTACT ====================
            builder.Property(x => x.Location)
                .HasMaxLength(200);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            // ==================== OPERATING HOURS ====================
            builder.Property(x => x.Is24Hours)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.OpeningTime)
                .IsRequired();

            builder.Property(x => x.ClosingTime)
                .IsRequired();

            // ==================== SORTING ====================
            builder.Property(x => x.SortOrder)
                .IsRequired()
                .HasDefaultValue(Department.DefaultSortOrder);

            // Index cho truy vấn sắp xếp
            builder.HasIndex(x => x.SortOrder);

            // ==================== STATUS ====================
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(x => x.IsActive);
            
            builder.Property(x => x.OnlineBookingEnabled)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
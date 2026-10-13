using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(x => x.Id);

            // ==================== FOREIGN KEYS ====================
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.DepartmentId).IsRequired();

            builder.Property(x => x.DoctorCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.DoctorCode).IsUnique();

            // ==================== PROFESSIONAL INFO ====================
            builder.Property(x => x.Title)
                .HasMaxLength(50);

            builder.Property(x => x.Specialization)
                .HasMaxLength(200);

            builder.Property(x => x.YearsOfExperience)
                .IsRequired(false);

            builder.Property(x => x.Biography)
                .HasMaxLength(2000);

            builder.Property(x => x.Languages)
                .HasMaxLength(100)
                .IsUnicode(false);

            // ==================== PERSONAL INFO ====================
            builder.Property(x => x.Avatar)
                .HasMaxLength(500);

            builder.Property(x => x.FullAddress)
                .HasMaxLength(500);

            builder.Property(x => x.Gender)
                .IsRequired(false);

            builder.Property(x => x.DateOfBirth)
                .IsRequired(false);

            builder.Property(x => x.IdentityNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.HasIndex(x => x.IdentityNumber)
                .IsUnique()
                .HasFilter("[IdentityNumber] IS NOT NULL");

            // ==================== LICENSE ====================
            builder.Property(x => x.LicenseNumber)
                .HasMaxLength(50);

            builder.HasIndex(x => x.LicenseNumber)
                .IsUnique()
                .HasFilter("[LicenseNumber] IS NOT NULL");

            builder.Property(x => x.LicenseIssueDate)
                .IsRequired(false);

            builder.Property(x => x.LicenseExpiryDate)
                .IsRequired(false);

            builder.Property(x => x.LicenseIssuedBy)
                .HasMaxLength(200);

            // ==================== FINANCIAL ====================
            builder.Property(x => x.ConsultationFee)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            // ==================== WORKLOAD ====================
            builder.Property(x => x.MaxPatientsPerDay)
                .IsRequired(false);

            builder.Property(x => x.JoinDate)
                .IsRequired(false);

            // ==================== DESCRIPTION ====================
            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            // ==================== STATUS ====================
            builder.Property(x => x.IsAvailableForOnline)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.SortOrder)
                .IsRequired()
                .HasDefaultValue(Doctor.DefaultSortOrder);

            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => x.SortOrder);

            // ==================== RELATIONSHIPS ====================
            // Department: Restrict (không cho xóa Department khi còn Doctor)
            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Account: Cascade (xóa Account → xóa Doctor)
            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
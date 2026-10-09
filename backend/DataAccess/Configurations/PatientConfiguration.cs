using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.Id);

            // ==================== PROPERTIES ====================
            builder.Property(p => p.PatientCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(p => p.IdentityNumber)
                .HasMaxLength(20);

            builder.Property(p => p.HealthInsuranceNumber)
                .HasMaxLength(20);

            builder.Property(p => p.IsPrimary)
                .IsRequired();

            // Gender là bool? — true=Nam, false=Nữ, null=Chưa xác định
            builder.Property(p => p.Gender)
                .IsRequired(false);

            builder.Property(p => p.DateOfBirth)
                .IsRequired(false);

            // ==================== ADDRESS VALUE OBJECT ====================
            // OwnsOne: Address lưu cùng bảng Patients với prefix cột
            builder.OwnsOne(p => p.Address, a =>
            {
                a.Property(x => x.Tinh)
                    .HasColumnName("Tinh")
                    .HasMaxLength(100);

                a.Property(x => x.Xa)
                    .HasColumnName("Xa")
                    .HasMaxLength(100);

                a.Property(x => x.Duong)
                    .HasColumnName("Duong")
                    .HasMaxLength(200);

                a.Property(x => x.SoNha)
                    .HasColumnName("SoNha")
                    .HasMaxLength(50);

                // FullText là computed property → KHÔNG map cột
                a.Ignore(x => x.FullText);
            });

            // ==================== INDEXES ====================
            // Mã bệnh nhân unique
            builder.HasIndex(p => p.PatientCode)
                .IsUnique();

            // CCCD unique nhưng cho phép nhiều NULL
            builder.HasIndex(p => p.IdentityNumber)
                .IsUnique()
                .HasFilter("[IdentityNumber] IS NOT NULL");

            // BHYT unique nhưng cho phép nhiều NULL
            builder.HasIndex(p => p.HealthInsuranceNumber)
                .IsUnique()
                .HasFilter("[HealthInsuranceNumber] IS NOT NULL");

            // Index cho query thường dùng
            builder.HasIndex(p => p.AccountId);
            builder.HasIndex(p => p.PrimaryPatientId);
            builder.HasIndex(p => p.FullName);

            // ==================== RELATIONSHIPS ====================

            // 1. Patient → Account (0..1)
            // Khi xóa Account → SetNull (Patient vẫn tồn tại)
            builder.HasOne(p => p.Account)
                .WithMany()
                .HasForeignKey(p => p.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            // 2. Patient → Patient (self-reference, 1-N)
            // PrimaryPatient → Dependents
            // Restrict: không cho xóa Patient khi còn Dependents
            builder.HasOne(p => p.PrimaryPatient)
                .WithMany(p => p.Dependents)
                .HasForeignKey(p => p.PrimaryPatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Patient → PatientDelegation (1-N)
            // Cấu hình trong PatientDelegationConfiguration (WithMany ở đây)
            // Nhưng vẫn cần báo EF Core biết backing field
            builder.Metadata
                .FindNavigation(nameof(Patient.Dependents))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata
                .FindNavigation(nameof(Patient.Delegations))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
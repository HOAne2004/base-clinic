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

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PatientCode)
                .IsRequired()
                .HasMaxLength(20);
            builder.HasIndex(x => x.PatientCode).IsUnique();

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.IdentityNumber)
                .HasMaxLength(20);
            builder.HasIndex(x => x.IdentityNumber)
                .IsUnique()
                .HasFilter("[IdentityNumber] IS NOT NULL");

            builder.Property(x => x.Avatar).HasMaxLength(500);
            builder.Property(x => x.FullAddress).HasMaxLength(500);

            // Quan hệ với Account
            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ đệ quy: Bệnh nhân phụ trỏ về Bệnh nhân chính
            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PrimaryPatientId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa bệnh nhân chính nếu còn bệnh nhân phụ
        }
    }
}

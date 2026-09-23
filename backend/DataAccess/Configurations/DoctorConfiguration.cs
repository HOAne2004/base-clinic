using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BaseClinic.Domain.Entities;

namespace BaseClinic.DataAccess.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(x => x.Id);

            // Cấu hình các khóa ngoại
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.DepartmentId).IsRequired();

            // Đảm bảo DoctorCode là duy nhất
            builder.Property(x => x.DoctorCode)
                .IsRequired()
                .HasMaxLength(50);
            builder.HasIndex(x => x.DoctorCode).IsUnique();

            builder.Property(x => x.Avatar)
                .HasMaxLength(500);

            builder.Property(x => x.FullAddress)
                .HasMaxLength(500);

            // Cấu hình Unique Index cho CCCD/CMND và Chứng chỉ hành nghề (nếu có nhập)
            builder.Property(x => x.IdentityNumber).HasMaxLength(20);
            builder.HasIndex(x => x.IdentityNumber).IsUnique().HasFilter("[IdentityNumber] IS NOT NULL");

            builder.Property(x => x.LicenseNumber).HasMaxLength(50);
            builder.HasIndex(x => x.LicenseNumber).IsUnique().HasFilter("[LicenseNumber] IS NOT NULL");

            // Cấu hình kiểu tiền tệ cho phí khám
            builder.Property(x => x.ConsultationFee)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            // Mapping Relationships (Tùy thuộc vào việc Account/Department có Navigation Properties hay không)
            // Thiết lập Restrict cho Department để tránh vô tình xóa Department khi vẫn còn Doctor
            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

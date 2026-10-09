using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
    {
        public void Configure(EntityTypeBuilder<LoginHistory> builder)
        {
            builder.ToTable("LoginHistories");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.IpAddress)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(l => l.DeviceInfo)
                .HasMaxLength(500);

            builder.Property(l => l.FailureReason)
                .HasMaxLength(500);

            builder.Property(l => l.IsSuccess)
                .IsRequired();

            builder.Property(l => l.LoginAt)
                .IsRequired();

            // Index cho query lịch sử theo Account + thời gian
            builder.HasIndex(l => new { l.AccountId, l.LoginAt });
        }
    }
}
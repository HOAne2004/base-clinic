using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
    {
        public void Configure(EntityTypeBuilder<OtpCode> builder)
        {
            builder.ToTable("OtpCodes");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            builder.Property(o => o.Purpose)
                .IsRequired();

            builder.Property(o => o.Channel)
                .IsRequired();

            builder.Property(o => o.Attempts)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(o => o.MaxAttempts)
                .IsRequired()
                .HasDefaultValue(5);

            // Index cho query OTP còn hiệu lực
            builder.HasIndex(o => new { o.AccountId, o.Purpose, o.ExpiresAt });
        }
    }
}
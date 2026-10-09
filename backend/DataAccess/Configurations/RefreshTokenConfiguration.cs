using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.TokenHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.DeviceInfo)
                .HasMaxLength(500);

            builder.Property(r => r.IpAddress)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(r => r.UserAgent)
                .HasMaxLength(500);

            builder.Property(r => r.RevokeReason)
                .HasMaxLength(500);

            builder.Property(r => r.IsTrusted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(r => r.AccountId);
            builder.HasIndex(r => r.TokenHash)
                .IsUnique();

            // Relationship cấu hình ở AccountConfiguration
        }
    }
}
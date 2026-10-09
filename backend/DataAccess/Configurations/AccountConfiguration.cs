using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);

            // ==================== IDENTITY ====================
            builder.Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Email)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(a => a.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(a => a.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            // ==================== PROFILE ====================
            builder.Property(a => a.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(a => a.PreferredLanguage)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("vi");

            // ==================== STATUS ====================
            builder.Property(a => a.Status)
                .IsRequired();

            builder.Property(a => a.IsEmailVerified)
                .IsRequired();

            builder.Property(a => a.IsPhoneNumberVerified)
                .IsRequired();

            // ==================== SECURITY ====================
            builder.Property(a => a.TwoFactorEnabled)
                .IsRequired();

            builder.Property(a => a.MustChangePasswordOnNextLogin)
                .IsRequired();

            // ==================== DEACTIVATION ====================
            builder.Property(a => a.DeactivationReason)
                .HasMaxLength(500);

            // ==================== INDEXES ====================
            builder.HasIndex(a => a.PhoneNumber)
                .IsUnique();

            builder.HasIndex(a => a.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

            builder.HasIndex(a => a.Status);

            // ==================== RELATIONSHIPS ====================

            // 1. Account → AccountRole (1-N)
            builder.HasMany(x => x.AccountRoles)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Account.AccountRoles))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 2. Account → RefreshToken (1-N)
            builder.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Account.RefreshTokens))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 3. Account → LoginHistory (1-N)
            builder.HasMany(x => x.LoginHistories)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Account.LoginHistories))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 4. Account → OtpCode (1-N)
            builder.HasMany(x => x.OtpCodes)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Account.OtpCodes))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
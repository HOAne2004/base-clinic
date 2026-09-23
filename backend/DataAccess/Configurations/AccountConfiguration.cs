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
            
            builder.Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(a => a.Email)
                .HasMaxLength(255)
                .IsRequired(false);
            
            builder.Property(a => a.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(a => a.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            
            builder.Property(a => a.Status)
                .IsRequired();
            
            builder.Property(a => a.IsEmailVerified)
                .IsRequired();
            
            builder.Property(a => a.IsPhoneNumberVerified)
                .IsRequired();
            
            builder.Property(a => a.LastLoginAt)
                .IsRequired(false);
            
            builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

            // Mapping Navigation property (Backing field)
            builder.HasMany(x => x.AccountRoles)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Báo cho EF Core biết sử dụng backing field _accountRoles cho collection AccountRoles
            builder.Metadata.FindNavigation(nameof(Account.AccountRoles))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

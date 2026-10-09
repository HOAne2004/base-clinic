using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class AccountRoleConfiguration : IEntityTypeConfiguration<AccountRole>
    {
        public void Configure(EntityTypeBuilder<AccountRole> builder)
        {
            builder.ToTable("AccountRoles");

            builder.HasKey(ar => new { ar.AccountId, ar.RoleId });

            builder.Property(ar => ar.AssignedAt)
                .IsRequired();

            builder.HasOne(ar => ar.Account)
                .WithMany(a => a.AccountRoles)      // ⚡ Chỉ định rõ navigation
                .HasForeignKey(ar => ar.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ar => ar.Role)
                .WithMany(r => r.AccountRoles)      // ⚡ Chỉ định rõ navigation
                .HasForeignKey(ar => ar.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
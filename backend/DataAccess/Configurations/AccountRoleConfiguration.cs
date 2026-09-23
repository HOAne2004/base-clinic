using Microsoft.EntityFrameworkCore;
using BaseClinic.Domain.Entities;

namespace BaseClinic.DataAccess.Configurations
{
    public class AccountRoleConfiguration : IEntityTypeConfiguration<AccountRole>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AccountRole> builder)
        {
            builder.ToTable("AccountRoles");

            builder.HasKey(ar => new { ar.AccountId, ar.RoleId });

            builder.HasOne(ar => ar.Account)
                .WithMany(a => a.AccountRoles)
                .HasForeignKey(ar => ar.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ar => ar.Role)
                .WithMany()
                .HasForeignKey(ar => ar.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

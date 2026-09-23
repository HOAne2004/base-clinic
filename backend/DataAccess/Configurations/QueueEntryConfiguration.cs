using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
    {
        public void Configure(EntityTypeBuilder<QueueEntry> builder)
        {
            builder.ToTable("QueueEntries");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.QueueNumber)
                .IsRequired();

            // Ràng buộc 1 Encounter chỉ có 1 vị trí trong 1 Hàng đợi cụ thể
            builder.HasIndex(x => new { x.QueueId, x.EncounterId }).IsUnique();

            builder.HasOne<Queue>()
                .WithMany(q => q.QueueEntries)
                .HasForeignKey(x => x.QueueId)
                .OnDelete(DeleteBehavior.Cascade); // Queue bị hủy/xóa thì Entry bên trong cũng mất

            builder.HasOne<Encounter>()
                .WithMany()
                .HasForeignKey(x => x.EncounterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

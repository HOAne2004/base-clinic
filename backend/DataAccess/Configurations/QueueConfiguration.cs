using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class QueueConfiguration : IEntityTypeConfiguration<Queue>
    {
        public void Configure(EntityTypeBuilder<Queue> builder)
        {
            builder.ToTable("Queues");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.QueueCode)
                .IsRequired()
                .HasMaxLength(20);
            builder.HasIndex(x => x.QueueCode).IsUnique();

            builder.Property(x => x.QueueDate).HasColumnType("date");

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa khoa thì xóa hàng đợi của khoa đó

            // Cấu hình Backing Field cho Collection
            builder.Metadata.FindNavigation(nameof(Queue.QueueEntries))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

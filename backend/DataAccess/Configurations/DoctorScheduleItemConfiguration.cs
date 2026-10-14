using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class DoctorScheduleItemConfiguration
        : IEntityTypeConfiguration<DoctorScheduleItem>
    {
        public void Configure(EntityTypeBuilder<DoctorScheduleItem> builder)
        {
            builder.ToTable("DoctorScheduleItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoctorScheduleId).IsRequired();

            // ==================== TIME ====================
            builder.Property(x => x.StartTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(x => x.EndTime)
                .HasColumnType("time")
                .IsRequired();

            // ==================== CAPACITY ====================
            builder.Property(x => x.Capacity).IsRequired();
            builder.Property(x => x.BookedCount).IsRequired();

            // ==================== STATUS ====================
            builder.Property(x => x.Status).IsRequired();

            // ==================== CONCURRENCY ====================
            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsRequired();

            // ==================== INDEXES ====================
            // Query slot theo schedule + thời gian
            builder.HasIndex(x => new { x.DoctorScheduleId, x.StartTime });

            // Query slot available
            builder.HasIndex(x => x.Status);
        }
    }
}
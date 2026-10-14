using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.ToTable("DoctorSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoctorId).IsRequired();

            // ==================== TIME ====================
            builder.Property(x => x.WorkDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.StartTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(x => x.EndTime)
                .HasColumnType("time")
                .IsRequired();

            // ==================== CONFIG ====================
            builder.Property(x => x.SlotDurationMinutes).IsRequired();
            builder.Property(x => x.MaxPatientPerSlot).IsRequired();

            builder.Property(x => x.OnlineBookingEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.Note)
                .HasMaxLength(500);

            // ==================== STATUS ====================
            builder.Property(x => x.Status).IsRequired();

            builder.Property(x => x.CancelReason)
                .HasMaxLength(500);

            // ==================== RELATIONSHIPS ====================
            builder.HasOne(x => x.Doctor)
                .WithMany()
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.DoctorSchedule)
                .HasForeignKey(x => x.DoctorScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Backing field access
            builder.Metadata.FindNavigation(nameof(DoctorSchedule.Items))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // ==================== INDEXES ====================
            builder.HasIndex(x => new { x.DoctorId, x.WorkDate });
            builder.HasIndex(x => x.Status);
        }
    }
}
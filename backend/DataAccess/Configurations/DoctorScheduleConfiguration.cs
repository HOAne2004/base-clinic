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
                .IsRequired();

            builder.Property(x => x.EndTime)
                .IsRequired();

            builder.Property(x => x.SlotDurationMinutes)
                .IsRequired();

            builder.Property(x => x.MaxPatientPerSlot)
                .IsRequired();

            builder.Property(x => x.OnlineBookingCapacity)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            // ==================== RELATIONSHIPS ====================
            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== INDEXES ====================
            // Query nhanh theo doctor + ngày
            builder.HasIndex(x => new { x.DoctorId, x.WorkDate });

            // ⚡ CHỐNG TRÙNG CA:
            // 1 doctor không thể có 2 ca cùng ngày bắt đầu cùng giờ.
            // Nếu cho phép overlap (tăng capacity) → bỏ unique này.
            // Nếu chặn overlap → giữ.
            //
            // ⚠️ Bạn đã chọn "tăng capacity" khi overlap.
            // → KHÔNG dùng unique index.
            // → Chỉ dùng index thường để query.
            builder.HasIndex(x => new { x.DoctorId, x.WorkDate, x.StartTime });
        }
    }
}
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

            // Cấu hình kiểu dữ liệu ngày giờ
            builder.Property(x => x.WorkDate).HasColumnType("date").IsRequired();
            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.EndTime).IsRequired();

            builder.Property(x => x.SlotDurationMinutes).IsRequired();
            builder.Property(x => x.MaxPatientPerSlot).IsRequired();
            builder.Property(x => x.OnlineBookingCapacity).IsRequired();

            // Enum Status thường được lưu dưới dạng int
            builder.Property(x => x.Status).IsRequired();

            // Ràng buộc liên kết với Doctor
            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index hỗ trợ truy vấn nhanh theo Doctor và Ngày
            builder.HasIndex(x => new { x.DoctorId, x.WorkDate });
        }
    }
}

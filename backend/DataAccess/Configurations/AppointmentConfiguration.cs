using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.CancelReason)
                .HasMaxLength(500);

            builder.Property(x => x.AppointmentDate).HasColumnType("date");

            // EF Core 8+ hỗ trợ natively kiểu TimeOnly cho SQL Server / PostgreSQL
            builder.Property(x => x.StartTime);
            builder.Property(x => x.EndTime);

            // Khóa ngoại
            builder.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(x => x.RequestedDoctorId)
                .OnDelete(DeleteBehavior.SetNull); // Bác sĩ nghỉ việc vẫn giữ được lịch sử lịch hẹn
        }
    }
}

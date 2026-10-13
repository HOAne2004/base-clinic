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

            builder.HasKey(a => a.Id);

            // ==================== PROPERTIES ====================
            builder.Property(a => a.AppointmentCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.AppointmentDate)
                .IsRequired();

            builder.Property(a => a.StartTime)
                .IsRequired();

            builder.Property(a => a.EndTime)
                .IsRequired();

            // Snapshot contact
            builder.Property(a => a.ContactPhone)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(a => a.ContactName)
                .HasMaxLength(100);

            builder.Property(a => a.ContactRelationship)
                .IsRequired(false);

            builder.Property(a => a.Reason)
                .HasMaxLength(500);

            builder.Property(a => a.Status)
                .IsRequired();

            builder.Property(a => a.CancelReason)
                .HasMaxLength(500);

            // ==================== INDEXES ====================
            builder.HasIndex(a => a.AppointmentCode)
                .IsUnique();

            // Index cho query lịch hẹn theo ngày + bác sĩ
            builder.HasIndex(a => new { a.AppointmentDate, a.DoctorId });

            // Index cho query lịch hẹn theo patient
            builder.HasIndex(a => a.PatientId);

            // Index cho query lịch hẹn theo status
            builder.HasIndex(a => a.Status);

            // ==================== RELATIONSHIPS ====================

            // 1. Appointment → Patient (N-1, bắt buộc)
            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Appointment → Doctor (bác sĩ thực sự khám, N-1, optional)
            builder.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Appointment → RequestedDoctor (bác sĩ user yêu cầu, N-1, optional)
            // ⚡ CẦN chỉ định rõ navigation vì có 2 FK đến cùng bảng Doctors
            builder.HasOne(a => a.RequestedDoctor)
                .WithMany()
                .HasForeignKey(a => a.RequestedDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Appointment → Department (N-1, optional)
            builder.HasOne(a => a.Department)
                .WithMany()
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
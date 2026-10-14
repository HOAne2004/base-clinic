using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class ScheduleCancellationRequestConfiguration
        : IEntityTypeConfiguration<ScheduleCancellationRequest>
    {
        public void Configure(EntityTypeBuilder<ScheduleCancellationRequest> builder)
        {
            builder.ToTable("ScheduleCancellationRequests");

            builder.HasKey(x => x.Id);

            // ==================== PROPERTIES ====================
            builder.Property(x => x.DoctorScheduleId).IsRequired();

            builder.Property(x => x.RequestedByAccountId).IsRequired();

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.ReviewNote)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            // ==================== INDEXES ====================
            builder.HasIndex(x => x.DoctorScheduleId);
            builder.HasIndex(x => x.RequestedByAccountId);
            builder.HasIndex(x => x.Status);

            // ==================== RELATIONSHIPS ====================

            // FK → DoctorSchedule (cascade delete khi xóa schedule)
            builder.HasOne(x => x.DoctorSchedule)
                .WithMany()
                .HasForeignKey(x => x.DoctorScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK → Requester Account (restrict — không xóa account khi còn request)
            builder.HasOne(x => x.RequestedByAccount)
                .WithMany()
                .HasForeignKey(x => x.RequestedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK → Reviewer Account (optional, set null khi account bị xóa)
            builder.HasOne(x => x.ReviewedByAccount)
                .WithMany()
                .HasForeignKey(x => x.ReviewedByAccountId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
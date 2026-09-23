using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PrescriptionCode)
                .IsRequired()
                .HasMaxLength(30);
            builder.HasIndex(x => x.PrescriptionCode).IsUnique();

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.IssuedDate).HasColumnType("date");
            builder.Property(x => x.ValidUntil).HasColumnType("date");

            // Khóa ngoại trỏ về phiên khám
            builder.HasOne<Consultation>()
                .WithMany()
                .HasForeignKey(x => x.ConsultationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình Backing Field cho Collection để EF Core có thể map dữ liệu vào Aggregate Root
            builder.Metadata.FindNavigation(nameof(Prescription.PrescriptionItems))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

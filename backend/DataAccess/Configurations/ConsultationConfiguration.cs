using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
    {
        public void Configure(EntityTypeBuilder<Consultation> builder)
        {
            builder.ToTable("Consultations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DiagnosisCode)
                .HasMaxLength(20);

            builder.Property(x => x.DiagnosisName)
                .HasMaxLength(255);

            builder.Property(x => x.TreatmentPlan)
                .HasMaxLength(1000);

            builder.HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa bác sĩ nếu đã có lịch sử khám

            builder.HasOne<Encounter>()
                .WithMany()
                .HasForeignKey(x => x.EncounterId)
                .OnDelete(DeleteBehavior.Cascade); // Nếu lượt khám bị hủy/xóa, phiên chẩn đoán cũng đi theo
        }
    }
}

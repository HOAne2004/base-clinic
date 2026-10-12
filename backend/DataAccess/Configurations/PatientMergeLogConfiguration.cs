using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class PatientMergeLogConfiguration : IEntityTypeConfiguration<PatientMergeLog>
    {
        public void Configure(EntityTypeBuilder<PatientMergeLog> builder)
        {
            builder.ToTable("PatientMergeLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MergedSnapshot)
                .IsRequired()
                .HasColumnType("nvarchar(max)");   // JSON có thể dài

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.RollbackReason)
                .HasMaxLength(500);

            builder.HasIndex(x => x.SurvivorPatientId);
            builder.HasIndex(x => x.MergedPatientId);
            builder.HasIndex(x => x.MergedAt);

            // FK
            builder.HasOne(x => x.SurvivorPatient)
                .WithMany()
                .HasForeignKey(x => x.SurvivorPatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // MergedPatientId không có FK constraint vì Patient có thể bị xóa cứng sau này
        }
    }
}
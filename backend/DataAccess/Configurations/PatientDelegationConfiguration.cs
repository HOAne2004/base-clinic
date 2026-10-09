using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class PatientDelegationConfiguration : IEntityTypeConfiguration<PatientDelegation>
    {
        public void Configure(EntityTypeBuilder<PatientDelegation> builder)
        {
            builder.ToTable("PatientDelegations");

            builder.HasKey(d => d.Id);

            // ==================== PROPERTIES ====================
            builder.Property(d => d.RelationshipType)
                .IsRequired();

            builder.Property(d => d.AccessLevel)
                .IsRequired();

            builder.Property(d => d.Status)
                .IsRequired();

            builder.Property(d => d.RevokeReason)
                .HasMaxLength(500);

            // ==================== INDEXES ====================
            builder.HasIndex(d => d.TargetPatientId);
            builder.HasIndex(d => d.ObserverAccountId);
            builder.HasIndex(d => d.Status);

            // ⚡ UNIQUE FILTERED INDEX:
            // Đảm bảo 1 Account chỉ có TỐI ĐA 1 delegation "còn sống" với 1 Patient.
            // Status = 1 (Pending) HOẶC 2 (Accepted) → unique.
            // Revoked (4) / Rejected (3) → cho phép tạo lại.
            builder.HasIndex(d => new { d.TargetPatientId, d.ObserverAccountId })
                .HasFilter("[Status] IN (1, 2)")
                .IsUnique();

            // ==================== RELATIONSHIPS ====================

            // 1. PatientDelegation → Patient (Target)
            // Cascade: xóa Patient → xóa hết delegations của nó
            builder.HasOne(d => d.TargetPatient)
                .WithMany(p => p.Delegations)
                .HasForeignKey(d => d.TargetPatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. PatientDelegation → Account (Observer)
            // Cascade: xóa Account → xóa hết delegations của nó
            builder.HasOne(d => d.ObserverAccount)
                .WithMany()
                .HasForeignKey(d => d.ObserverAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lưu ý: RequestedByAccountId và ApprovedByAccountId
            // KHÔNG có navigation → chỉ là Guid? tham chiếu audit.
            // Không cấu hình FK để tránh phức tạp.

            // Backing field access
            builder.Metadata
                .FindNavigation(nameof(Patient.Delegations))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
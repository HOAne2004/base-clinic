using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class MergeRollbackRequestConfiguration
        : IEntityTypeConfiguration<MergeRollbackRequest>
    {
        public void Configure(EntityTypeBuilder<MergeRollbackRequest> builder)
        {
            builder.ToTable("MergeRollbackRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ReviewNote)
                .HasMaxLength(500);

            builder.Property(x => x.ConflictResolutions)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasIndex(x => x.PatientMergeLogId);
            builder.HasIndex(x => x.RequestedByAccountId);
            builder.HasIndex(x => x.Status);

            // FK to PatientMergeLog
            builder.HasOne(x => x.PatientMergeLog)
                .WithMany()
                .HasForeignKey(x => x.PatientMergeLogId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK to Requester
            builder.HasOne(x => x.RequestedByAccount)
                .WithMany()
                .HasForeignKey(x => x.RequestedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK to Reviewer (optional)
            builder.HasOne(x => x.ReviewedByAccount)
                .WithMany()
                .HasForeignKey(x => x.ReviewedByAccountId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
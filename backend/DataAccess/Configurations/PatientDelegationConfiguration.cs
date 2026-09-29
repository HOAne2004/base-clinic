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

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TargetPatientId)
                .IsRequired();

            builder.Property(x => x.ObserverAccountId)
                .IsRequired();

            builder.Property(x => x.RelationshipType)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            // Đánh Index để tối ưu hóa hiệu năng cho câu lệnh kiểm tra quyền (thường xuyên query theo 2 trường này)
            builder.HasIndex(x => new { x.TargetPatientId, x.ObserverAccountId });

            // Index để lấy danh sách người thân nhanh hơn (khi query: "Tìm những người tôi đang theo dõi")
            builder.HasIndex(x => x.ObserverAccountId);
        }
    }
}
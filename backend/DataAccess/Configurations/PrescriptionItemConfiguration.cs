using BaseClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseClinic.DataAccess.Configurations
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");
            builder.HasKey(x => x.Id);

            // Ràng buộc: Một loại thuốc không được xuất hiện 2 lần trong cùng 1 đơn
            builder.HasIndex(x => new { x.PrescriptionId, x.MedicineId }).IsUnique();

            builder.Property(x => x.MedicineNameSnapShot)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Unit)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Instructions)
                .HasMaxLength(500);

            // Liên kết với Đơn thuốc (Cascade Delete: Xóa đơn thuốc -> xóa luôn chi tiết thuốc)
            builder.HasOne<Prescription>()
                .WithMany(p => p.PrescriptionItems)
                .HasForeignKey(x => x.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Liên kết với Danh mục thuốc (Restrict: Không cho xóa thuốc khỏi danh mục nếu đã kê vào đơn)
            builder.HasOne<Medicine>()
                .WithMany()
                .HasForeignKey(x => x.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

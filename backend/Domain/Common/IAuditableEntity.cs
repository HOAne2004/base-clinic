namespace BaseClinic.Domain.Common
{
    // Các thuộc tính này được ClinicDbContext tự động điền khi SaveChangesAsync().
    public interface IAuditableEntity : IEntity
    {
        DateTimeOffset CreatedDate { get; }
        string? CreatedBy { get; } //tên hiển thị (dễ đọc log)
        Guid? CreatedId { get; } //FK thật (dùng để join khi cần)
        DateTimeOffset UpdatedDate { get; }
        string? UpdatedBy { get; }
        Guid? UpdatedId { get; }
    }
}

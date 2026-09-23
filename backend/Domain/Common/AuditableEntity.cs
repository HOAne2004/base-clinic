namespace BaseClinic.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset? UpdatedAt { get; protected set; }
        public Guid? CreatedBy { get; protected set; }
        public Guid? UpdatedBy { get; protected set; }
    }
}

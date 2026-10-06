namespace BaseClinic.Domain.Common
{
    public class AuditableEntity : Entity, IAuditableEntity
    {
        public DateTimeOffset CreatedDate { get; protected set; }
        public string? CreatedBy { get; protected set; }
        public Guid? CreatedId { get; protected set; }
        public DateTimeOffset UpdatedDate { get; protected set; }
        public string? UpdatedBy { get; protected set; }
        public Guid? UpdatedId { get; protected set; }
    }
}

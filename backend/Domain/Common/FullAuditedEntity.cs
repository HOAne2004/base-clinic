namespace BaseClinic.Domain.Common
{
    public class FullAuditedEntity : AuditableEntity, ISoftDeleteEntity
    {
        public bool IsDeleted { get; protected set; }
        public DateTimeOffset? DeletedDate { get; protected set; }
        public Guid? DeletedId { get; protected set; }

        public void Restore()
        {
            IsDeleted = false;
            DeletedDate = null;
            DeletedId = null;
        }
    }
}

namespace BaseClinic.Domain.Common
{
    public interface ISoftDeleteEntity : IEntity
    {
        bool IsDeleted { get; }
        DateTimeOffset? DeletedDate { get; }
        Guid? DeletedId { get; }

        /// <summary>
        /// Khôi phục bản ghi đã xóa mềm.
        /// Reset IsDeleted = false và xóa thông tin Deleted.
        /// </summary>
        void Restore();
    }
}

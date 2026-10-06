using BaseClinic.Domain.Common;

namespace BaseClinic.Business.Services
{
    public interface IService<T> where T : class, IEntity
    {
        // Lấy entity theo Id. Trả về null nếu không tìm thấy.
        Task<T?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default);

        // Lấy entity theo Id. NÉM EXCEPTION nếu không tìm thấy.
        // Tên entity trong message được lấy từ [DisplayName] attribute.
        Task<T> GetByIdOrThrowAsync(Guid? id, CancellationToken cancellationToken = default);

        // Tạo mới entity: Add + SaveAsync.
        Task CreateAsync(T entity, CancellationToken cancellationToken = default);

        // Cập nhật entity: Update + SaveAsync.
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        // Xóa entity: Delete + SaveAsync.
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task RestoreAsync(T entity, CancellationToken cancellationToken = default);
    }
}

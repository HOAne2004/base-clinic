using BaseClinic.Domain.Common;
using System.Linq.Expressions;

namespace BaseClinic.Business.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        // Lấy entity theo khóa chính
        // Trả về null nếu không tìm thấy (KHÔNG ném exception).
        Task<T?> GetByIdAsync(Guid? id);

        // Trả về IQueryable để truy vấn.
        // MẶC ĐỊNH bật AsNoTracking() — tối ưu cho đọc, KHÔNG dùng để sửa.
        IQueryable<T> GetQueryable();

        // Trả về IQueryable có bật Change Tracking — dùng khi cần đọc lên để sửa.
        IQueryable<T> GetQueryableWithTracking();

        // Tìm danh sách entity theo điều kiện (thực thi ngay, trả về IEnumerable).
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        // Trả về IQueryable đã lọc theo điều kiện (chưa thực thi — có thể chain tiếp).
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        // Đánh dấu thêm mới entity vào ChangeTracker.
        T Add(T entity);

        // Phiên bản async của Add — dùng khi cần await. Tuy nhien du an khong can vi:
        // Không dùng value generator nào cần I/O.
        // Giữ code đơn giản, đồng nhất với Update/Delete(đều sync).
        // Nếu tương lai cần, thêm lại dễ dàng.
        //Task<T> AddAsync(T entity);

        // Đánh dấu xóa entity.
        T Delete(T entity);

        // Khôi phục bản ghi đã xóa mềm.
        T Restore(T entity);

        // Đánh dấu entity ở trạng thái Modified.
        void Update(T entity);

        // Thực thi SaveChangesAsync() — commit toàn bộ thay đổi vào DB.
        // Đây là nơi ClinicDbContext tự động điền audit + xử lý soft delete.
        // Tuy nhiên đã có UnitOfWork nên không can triển khai trong dự án
        Task SaveAsync(CancellationToken cancellationToken = default);

        // Kiểm tra tồn tại bản ghi theo điều kiện.
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}

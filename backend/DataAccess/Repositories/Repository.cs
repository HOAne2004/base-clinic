using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BaseClinic.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly ClinicDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ClinicDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid? id)
            => await _dbSet.FindAsync(id);
        // FindAsync ưu tiên tìm trong ChangeTracker trước, nếu không có mới query DB.

        public IQueryable<T> GetQueryable() // Đã có global lọc xóa mềm 
            => _dbSet.AsNoTracking();
        // AsNoTracking: không theo dõi thay đổi → nhanh hơn, nhưng KHÔNG dùng để sửa.

        public IQueryable<T> GetQueryableWithTracking()
            => _dbSet.AsQueryable();
        // Có tracking — dùng khi cần đọc lên sửa rồi SaveAsync().

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
            => _dbSet.Where(predicate).ToList();
        // ToList() thực thi query ngay — trả về IEnumerable.

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
            => _dbSet.Where(predicate);
        // Chưa thực thi — có thể chain .OrderBy().Skip().Take()...

        public T Add(T entity)
            => _dbSet.Add(entity).Entity;

        //public async Task<T> AddAsync(T entity)
        //    => (await _dbSet.AddAsync(entity)).Entity;

        public T Delete(T entity)
            => _dbSet.Remove(entity).Entity;
        // Remove() chỉ đánh dấu Deleted. ClinicDbContext sẽ chuyển thành soft delete
        // cho entity implement ISoftDeleteEntity.

        public T Restore(T entity)
        {
            if (entity is not FullAuditedEntity fullAudited)
                throw new InvalidOperationException(
                    $"Entity {typeof(T).Name} không hỗ trợ khôi phục (không kế thừa FullAuditedEntity).");

            fullAudited.Restore();
            _dbSet.Update(entity);  // Đánh dấu Modified
            return entity;
        }

        public void Update(T entity)
            => _dbSet.Update(entity);

        public async Task SaveAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync();

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);
    }
}

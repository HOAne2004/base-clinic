using BaseClinic.Business.Interfaces;
using BaseClinic.Domain.Common;
using BaseClinic.Domain.Exeptions;
using System.ComponentModel;
using System.Reflection;

namespace BaseClinic.Business.Services
{
    public class Service<T> : IService<T> where T : class, IEntity
    {
        private readonly IRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Service(IRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<T?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
            => await _repository.GetByIdAsync(id);

        public async Task<T> GetByIdOrThrowAsync(Guid? id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                // Lấy tên hiển thị từ[DisplayName] của entity.
                // Ví dụ: [DisplayName("Bệnh nhân")] → "Không tìm thấy Bệnh nhân với Id 
                string entityName = typeof(T).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                                    ?? typeof(T).Name;
                throw new NotFoundException($"Không tìm thấy {entityName} với Id {id}");
            }
            return entity;
        }

        // Repository.SaveAsync: shortcut cho Service đơn giản chỉ dùng 1 repository.
        // UnitOfWork.SaveChangesAsync: dùng khi cần transaction across nhiều repository.
        // LƯU Ý: Cả hai đều gọi _context.SaveChangesAsync() — chọn 1 trong 2, không gọi cả hai.
        public virtual async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _repository.Add(entity);
            //await _repository.SaveAsync(cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _repository.Update(entity);
            //await _repository.SaveAsync(cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _repository.Delete(entity);
            //await _repository.SaveAsync(cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RestoreAsync(T entity, CancellationToken cancellationToken = default)
        {
            _repository.Restore(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

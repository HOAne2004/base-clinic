using BaseClinic.Business.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace BaseClinic.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;
        public UnitOfWork(ClinicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _currentTransaction?.Dispose();
        }
    }
}

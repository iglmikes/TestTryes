using DBAbstractions.implementation;
using DBAbstractions.Interfaces;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFPostgreDataProvider.Core
{
    internal sealed class PostgreUnitOfWork : IUnitOfWork
    {
        private readonly PostgreDbContext _context;
        private bool _disposed;

        public PostgreUnitOfWork(PostgreDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Users = new EfRepository<User>(_context);
            Orders = new EfRepository<Order>(_context);
        }

        public IRepository<User> Users { get; }
        public IRepository<Order> Orders { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _context.DisposeAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}

using DBAbstractions.implementation;
using DBAbstractions.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EFMySqlDataProvider.Core
{
    internal sealed class MySqlUnitOfWork : IUnitOfWork
    {
        private readonly MySqlDbContext _context;
        private bool _disposed;

        public MySqlUnitOfWork(MySqlDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Users = new EfRepository<User>(_context);
            Orders = new EfRepository<Order>(_context);
        }

        public IRepository<User> Users { get; }
        public IRepository<Order> Orders { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mySqlEx)
            {
                // Обработка специфичных ошибок MySQL
                throw new Exception($"MySQL error: {mySqlEx.Message}", mySqlEx);
            }
        }

        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            // MySQL требует особой обработки транзакций для некоторых движков
            if (_context.Database.IsMySql())
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                // BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return true;
                }
                catch (MySqlException ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new Exception("MySQL transaction failed", ex);
                }
            }

            return await SaveChangesAsync(cancellationToken) > 0;
        }

        public void Dispose() => DisposeAsync().AsTask().Wait();

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

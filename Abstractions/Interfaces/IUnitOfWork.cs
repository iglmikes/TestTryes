
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAbstractions.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Order> Orders { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<bool> CommitAsync(CancellationToken cancellationToken = default);
    }
}

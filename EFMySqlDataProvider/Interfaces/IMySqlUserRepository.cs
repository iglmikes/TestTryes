using DBAbstractions.Interfaces;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMySqlDataProvider.Interfaces
{
    public interface IMySqlUserRepository : IRepository<User>
    {
        Task BulkInsertAsync(IEnumerable<User> users);
        Task LockTableForWriteAsync();
        Task<IEnumerable<User>> GetUsersByFullTextSearch(string query);
    }
}

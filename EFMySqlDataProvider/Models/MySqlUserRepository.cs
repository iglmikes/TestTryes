using DBAbstractions.implementation;
using EFMySqlDataProvider.Core;
using EFMySqlDataProvider.Interfaces;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMySqlDataProvider.Models
{
    //internal class MySqlUserRepository : EfRepository<User>, IMySqlUserRepository
    //{
    //    private readonly MySqlDbContext _context;

    //    public MySqlUserRepository(MySqlDbContext context) : base(context)
    //    {
    //        _context = context;
    //    }

    //    // MySQL-специфичные методы
    //    public async Task BulkInsertAsync(IEnumerable<User> users)
    //    {
    //        await _context.Users.AddRangeAsync(users);
    //        await _context.Database.ExecuteSqlRawAsync(
    //            "SET FOREIGN_KEY_CHECKS = 0;"); // Отключаем проверки для быстрой вставки
    //    }

    //    public async Task LockTableForWriteAsync()
    //    {
    //        await _context.Database.ExecuteSqlRawAsync(
    //            "LOCK TABLES users WRITE;");
    //    }

    //    // Оптимизированные для MySQL запросы
    //    public async Task<IEnumerable<User>> GetUsersByFullTextSearch(string query)
    //    {
    //        return await _context.Users
    //            .FromSqlRaw(
    //                "SELECT * FROM users WHERE MATCH(name, email) AGAINST({0} IN BOOLEAN MODE)",
    //                query)
    //            .ToListAsync();
    //    }
    //}
}

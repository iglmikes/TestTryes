using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAbstractions.Interfaces
{
    /// <summary>
    /// Reg Service by DI
    /// </summary>
    public interface IDatabaseProvider
    {
        void ConfigureDbContext(IServiceCollection services, string connectionString);
        void RegisterUnitOfWork(IServiceCollection services);
    }
}

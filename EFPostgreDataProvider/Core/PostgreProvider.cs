using DBAbstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFPostgreDataProvider.Core
{
    public class PostgreProvider : IDatabaseProvider
    {
        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PostgreDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        public void RegisterUnitOfWork(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(provider =>
                new PostgreUnitOfWork(provider.GetRequiredService<PostgreDbContext>()));
        }
    }
}

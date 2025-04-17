using DBAbstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMySqlDataProvider.Core
{
    public class MySqlProvider : IDatabaseProvider
    {
        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MySqlDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysqlOptions =>
                    {
                        // Оптимизации для MySQL
                        mysqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);

                        mysqlOptions.CommandTimeout(300);
                    });

#if DEBUG
                options.EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
#endif
            });
        }

        public void RegisterUnitOfWork(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<MySqlDbContext>();
                return new MySqlUnitOfWork(context);
            });
        }


    }
}

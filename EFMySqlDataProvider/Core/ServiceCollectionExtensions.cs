
using DBAbstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;



namespace EFMySqlDataProvider.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMySqlUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(provider =>
                new MySqlUnitOfWork(provider.GetRequiredService<MySqlDbContext>()));
            return services;
        }
    }
}

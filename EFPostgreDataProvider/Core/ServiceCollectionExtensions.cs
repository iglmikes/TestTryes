
using DBAbstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;



namespace EFPostgreDataProvider.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(provider =>
                new PostgreUnitOfWork(provider.GetRequiredService<PostgreDbContext>()));
            return services;
        }
    }
}

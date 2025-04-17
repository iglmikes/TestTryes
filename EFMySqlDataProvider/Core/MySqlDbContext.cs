using DBAbstractions.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMySqlDataProvider.Core
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
            : base(options) { }

        internal DbSet<User> Users => Set<User>();
        internal DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация для MySQL
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.HasIndex(u => u.Email)
                .HasDatabaseName("idx_email") // Имя индекса
                .IsUnique();                  // Уникальность


                // Для MySQL специфичные настройки
                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");//.HasDefaultValueSql("UTC_TIMESTAMP()") // MySQL
            });

            //modelBuilder.Entity<Order>(entity =>
            //{
            //    entity.ToTable("orders");
            //    entity.HasIndex(o => o.UserId);

            //    // Пример отношения
            //    entity.HasOne(o => o.User)
            //        .WithMany(u => u.Orders)
            //        .HasForeignKey(o => o.UserId);
            //});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Опционально: логирование SQL (только для разработки)
#if DEBUG
            options.EnableDetailedErrors()
                   .EnableSensitiveDataLogging()
                   .LogTo(Console.WriteLine, LogLevel.Information);
#endif
        }

        public static void ConfigureServices(IServiceCollection services, string connectionString)
        {
            // Регистрация контекста
            services.AddDbContext<MySqlDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysqlOptions =>
                    {
                        mysqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null); // Явно указываем null для errorNumbersToAdd
                    });

#if DEBUG
                options.EnableDetailedErrors()
                       .EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine, LogLevel.Information);
#endif
            });

            // Регистрация UnitOfWork
            services.AddScoped<IUnitOfWork>(provider =>
                new MySqlUnitOfWork(provider.GetRequiredService<MySqlDbContext>()));

            // Убрана регистрация MySqlQueryBuilder (если не используется)
            // services.AddScoped<IMySqlQueryBuilder, MySqlQueryBuilder>();
        }



    }
}

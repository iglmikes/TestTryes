using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFPostgreDataProvider.Core
{
    public class PostgreDbContext : DbContext //этро для всех EF вроде должно работать - мб перенесть в ... крнкретную БД? Что останется
    {

        public static void ConfigureServices(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PostgreDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddPostgresUnitOfWork();
        }


        public PostgreDbContext(DbContextOptions<PostgreDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        // Можно где то отдельно доописать схему всей БД и при запуске валидировать и соотсношать(?)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .HasMaxLength(255)
                    .IsRequired();

                // ...
                entity.Property(u => u.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()") // Для PostgreSQL  //.HasDefaultValueSql("timezone('utc', now())") // PostgreSQL
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.UpdatedAt)
                    .ValueGeneratedOnUpdate();
            });
        }

    }
}

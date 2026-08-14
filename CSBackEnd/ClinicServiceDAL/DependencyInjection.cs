using ClinicServiceBase.DAL.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicServiceDAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClinicDAL(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

            var provider = configuration["Database:Provider"] ?? "mssql";

            services.AddDbContext<ClinicDbContext>(options => options.UseClinicDatabase(provider, connectionString));
            services.AddDbContext<SqliteClinicDbContext>(options => options.UseSqlite(connectionString));
            services.AddDbContext<MssqlClinicDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<PgsqlClinicDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
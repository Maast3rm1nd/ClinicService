using Microsoft.EntityFrameworkCore;

namespace ClinicServiceDAL
{
    public static class DbProviderExtensions
    {
        public static string[] Providers { get; } = { "mssql", "pgsql", "sqlite" };

        public static DbContextOptionsBuilder UseClinicDatabase(this DbContextOptionsBuilder options, string provider, string connectionString)
        {
            return provider.ToLowerInvariant() switch
            {
                "mssql" => options.UseSqlServer(connectionString),
                "pgsql" => options.UseNpgsql(connectionString),
                "sqlite" => options.UseSqlite(connectionString),
                _ => throw new ArgumentException($"Unknown database provider '{provider}'. Supported: {string.Join(", ", Providers)}")
            };
        }
    }
}
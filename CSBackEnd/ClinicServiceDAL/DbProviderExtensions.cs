using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ClinicServiceDAL
{
    public static class DbProviderExtensions
    {
        public static string[] Providers { get; } = { "mssql", "pgsql", "sqlite" };

        public static string DetectProvider(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Database connection string cannot be empty.", nameof(connectionString));
            }

            var parsed = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            var keys = parsed.Keys
                .Cast<string>()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (keys.Contains("Host")
                || keys.Contains("Username")
                || keys.Contains("Search Path")
                || keys.Contains("Ssl Mode")
                || (parsed.TryGetValue("Port", out var port) && string.Equals(port?.ToString(), "5432", StringComparison.Ordinal)))
            {
                return "pgsql";
            }

            if (keys.Contains("Initial Catalog")
                || keys.Contains("Integrated Security")
                || keys.Contains("Trusted_Connection")
                || keys.Contains("TrustServerCertificate"))
            {
                return "mssql";
            }

            if (keys.Contains("Data Source") || keys.Contains("Filename"))
            {
                return "sqlite";
            }

            if (keys.Contains("Server"))
            {
                return "mssql";
            }

            throw new ArgumentException(
                "The database provider could not be detected from the connection string. " +
                $"Supported providers: {string.Join(", ", Providers)}.",
                nameof(connectionString));
        }

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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicServiceDAL
{
    public class SqliteClinicDbContextFactory : IDesignTimeDbContextFactory<SqliteClinicDbContext>
    {
        public SqliteClinicDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("CLINIC_CONNECTION_STRING") ?? "Data Source=clinic.db";

            var optionsBuilder = new DbContextOptionsBuilder<SqliteClinicDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new SqliteClinicDbContext(optionsBuilder.Options);
        }
    }
}
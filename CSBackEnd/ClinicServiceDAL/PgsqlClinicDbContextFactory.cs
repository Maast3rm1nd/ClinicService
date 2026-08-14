using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicServiceDAL
{
    public class PgsqlClinicDbContextFactory : IDesignTimeDbContextFactory<PgsqlClinicDbContext>
    {
        public PgsqlClinicDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("CLINIC_CONNECTION_STRING")
                ?? "Host=localhost;Port=5432;Database=clinic;Username=postgres;Password=postgres";

            var optionsBuilder = new DbContextOptionsBuilder<PgsqlClinicDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new PgsqlClinicDbContext(optionsBuilder.Options);
        }
    }
}
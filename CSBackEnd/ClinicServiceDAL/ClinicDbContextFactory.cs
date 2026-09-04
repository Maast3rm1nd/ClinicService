using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicServiceDAL
{
    public class ClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
    {
        public ClinicDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("CLINIC_CONNECTION_STRING") ?? "Data Source=clinic.db";
            var provider = DbProviderExtensions.DetectProvider(connectionString);

            var optionsBuilder = new DbContextOptionsBuilder<ClinicDbContext>();
            optionsBuilder.UseClinicDatabase(provider, connectionString);

            return new ClinicDbContext(optionsBuilder.Options);
        }
    }
}
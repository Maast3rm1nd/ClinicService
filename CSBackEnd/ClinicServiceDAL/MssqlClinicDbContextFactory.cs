using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicServiceDAL
{
    public class MssqlClinicDbContextFactory : IDesignTimeDbContextFactory<MssqlClinicDbContext>
    {
        public MssqlClinicDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("CLINIC_CONNECTION_STRING")
                ?? "Server=localhost;Database=clinic;User Id=sa;Password=Your_password123;TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<MssqlClinicDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MssqlClinicDbContext(optionsBuilder.Options);
        }
    }
}
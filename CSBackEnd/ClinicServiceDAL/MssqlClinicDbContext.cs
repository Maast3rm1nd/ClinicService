namespace ClinicServiceDAL
{
    public class MssqlClinicDbContext : ClinicDbContext
    {
        public MssqlClinicDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<MssqlClinicDbContext> options)
            : base(new Microsoft.EntityFrameworkCore.DbContextOptions<ClinicDbContext>(options.Extensions.ToDictionary(e => e.GetType(), e => e)))
        {
        }
    }
}
namespace ClinicServiceDAL
{
    public class PgsqlClinicDbContext : ClinicDbContext
    {
        public PgsqlClinicDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<PgsqlClinicDbContext> options)
            : base(new Microsoft.EntityFrameworkCore.DbContextOptions<ClinicDbContext>(options.Extensions.ToDictionary(e => e.GetType(), e => e)))
        {
        }
    }
}
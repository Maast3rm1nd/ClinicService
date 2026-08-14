namespace ClinicServiceDAL
{
    public class SqliteClinicDbContext : ClinicDbContext
    {
        public SqliteClinicDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<SqliteClinicDbContext> options)
            : base(new Microsoft.EntityFrameworkCore.DbContextOptions<ClinicDbContext>(options.Extensions.ToDictionary(e => e.GetType(), e => e)))
        {
        }
    }
}
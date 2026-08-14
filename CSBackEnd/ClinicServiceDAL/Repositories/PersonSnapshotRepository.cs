using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class PersonSnapshotRepository : RepositoryBase<PersonSnapshot>, IPersonSnapshotRepository
    {
        public PersonSnapshotRepository(ClinicDbContext context) : base(context)
        {
        }
    }
}
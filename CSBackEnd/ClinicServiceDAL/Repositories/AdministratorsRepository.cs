using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class AdministratorsRepository : RepositoryBase<Administrator>, IAdministratorsRepository
    {
        public AdministratorsRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddAdministrator(Administrator administrator)
        {
            return AddObject(administrator);
        }

        public Task EditAdministrator(Administrator administrator)
        {
            return UpdateObject(administrator);
        }

        public Task<Administrator[]> GetAdministrators(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteAdministrator(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}
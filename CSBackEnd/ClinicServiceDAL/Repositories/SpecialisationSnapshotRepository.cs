using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class SpecialisationSnapshotRepository : RepositoryBase<SpecialisationSnapshot>, ISpecialisationSnapshotRepository
    {
        public SpecialisationSnapshotRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddSpecialisation(SpecialisationSnapshot specialisation)
        {
            return AddObject(specialisation);
        }

        public Task EditSpecialisation(SpecialisationSnapshot specialisation)
        {
            return UpdateObject(specialisation);
        }

        public Task<SpecialisationSnapshot[]> GetSpecialisations(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteSpecialisation(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}
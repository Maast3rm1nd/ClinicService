using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class MedicalCardSnapshotRepository : RepositoryBase<MedicalCardSnapshot>, IMedicalCardSnapshotRepository
    {
        public MedicalCardSnapshotRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddMedicalCard(MedicalCardSnapshot medicalCard)
        {
            return AddObject(medicalCard);
        }

        public Task EditMedicalCard(MedicalCardSnapshot medicalCard)
        {
            return UpdateObject(medicalCard);
        }

        public Task<MedicalCardSnapshot[]> GetMedicalCards(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteMedicalCard(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}
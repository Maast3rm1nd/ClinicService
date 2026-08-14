using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class DiagnosisSnapshotRepository : RepositoryBase<DiagnosisSnapshot>, IDiagnosisSnapshotRepository
    {
        public DiagnosisSnapshotRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddDiagnosis(DiagnosisSnapshot diagnosis)
        {
            return AddObject(diagnosis);
        }

        public Task EditDiagnosis(DiagnosisSnapshot diagnosis)
        {
            return UpdateObject(diagnosis);
        }

        public Task<DiagnosisSnapshot[]> GetDiagnoses(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteDiagnosis(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}
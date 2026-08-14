using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IDiagnosisSnapshotRepository : IClinicServiceRepository<DiagnosisSnapshot>
    {
        Task AddDiagnosis(DiagnosisSnapshot diagnosis);

        Task EditDiagnosis(DiagnosisSnapshot diagnosis);

        Task<DiagnosisSnapshot[]> GetDiagnoses(Guid[] ids, CancellationToken cancellationToken);

        Task DeleteDiagnosis(Guid id, CancellationToken cancellationToken);
    }
}
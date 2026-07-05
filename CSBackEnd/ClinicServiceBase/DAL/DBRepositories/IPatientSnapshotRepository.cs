using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IPatientSnapshotRepository : IClinicServiceRepository<PatientSnapshot>
    {
        Task AddPatient(PatientSnapshot patient);

        Task EditPatient(PatientSnapshot patient);

        Task<PersonSnapshot[]> GetPatients(Guid[] ids, CancellationToken cancellationToken);

        Task DeletePatient (Guid patientId, CancellationToken cancellationToken);
    }
}

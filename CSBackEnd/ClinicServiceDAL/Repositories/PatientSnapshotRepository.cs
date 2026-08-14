using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class PatientSnapshotRepository : RepositoryBase<PatientSnapshot>, IPatientSnapshotRepository
    {
        public PatientSnapshotRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddPatient(PatientSnapshot patient)
        {
            return AddObject(patient);
        }

        public Task EditPatient(PatientSnapshot patient)
        {
            return UpdateObject(patient);
        }

        public async Task<PersonSnapshot[]> GetPatients(Guid[] ids, CancellationToken cancellationToken)
        {
            var patients = await GetObjectsByIds(ids, cancellationToken);

            return patients.Select(p => new PersonSnapshot
            {
                Id = p.Id,
                FullName = p.FullName,
                ShortName = p.ShortName,
                CreationDateTime = p.CreationDateTime,
                EditDateTime = p.EditDateTime
            }).ToArray();
        }

        public Task DeletePatient(Guid patientId, CancellationToken cancellationToken)
        {
            return SoftDeleteById(patientId);
        }
    }
}
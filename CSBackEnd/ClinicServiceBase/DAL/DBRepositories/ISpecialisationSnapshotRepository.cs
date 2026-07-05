using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface ISpecialisationSnapshotRepository : IClinicServiceRepository<SpecialisationSnapshot>
    {
        Task AddSpecialisation(SpecialisationSnapshot specialisation);

        Task EditSpecialisation(SpecialisationSnapshot specialisation);

        Task<SpecialisationSnapshot[]> GetSpecialisations(Guid[] Ids, CancellationToken cancellationToken);

        Task DeleteSpecialisation(Guid id, CancellationToken cancellationToken);
    }
}

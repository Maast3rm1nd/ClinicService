using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IDoctorsRepository : IClinicServiceRepository<Doctor>
    {
        Task AddDoctor(Doctor doctor);

        Task EditDoctor(Doctor doctor);

        Task<Doctor[]> GetDoctors(Guid[] ids, CancellationToken cancellationToken);

        Task DeleteDoctor(Guid id, CancellationToken cancellationToken);
    }
}

using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IAdministratorsRepository : IClinicServiceRepository<Administrator>
    {
        Task AddAdministrator(Administrator administrator);

        Task EditAdministrator(Administrator administrator);

        Task<Administrator[]> GetAdministrators(Guid[] ids, CancellationToken cancellationToken);

        Task DeleteAdministrator (Guid id, CancellationToken cancellationToken);
    }
}

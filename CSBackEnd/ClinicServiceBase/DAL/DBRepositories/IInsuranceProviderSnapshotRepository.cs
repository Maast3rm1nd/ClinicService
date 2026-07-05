using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IInsuranceProviderSnapshotRepository : IClinicServiceRepository<InsuranceProviderSnapshot>
    {
        Task AddInsuranceProvider(InsuranceProviderSnapshot insuranceProvider);

        Task EditInsuranceProvider(InsuranceProviderSnapshot insuranceProvider);

        Task<InsuranceProviderSnapshot[]> GetInsuranceProviders(Guid[] ids, CancellationToken cancellationToken);

        Task DeleteInsuranceProvider(Guid id, CancellationToken cancellationToken);
    }
}

using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class InsuranceProviderSnapshotRepository : RepositoryBase<InsuranceProviderSnapshot>, IInsuranceProviderSnapshotRepository
    {
        public InsuranceProviderSnapshotRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddInsuranceProvider(InsuranceProviderSnapshot insuranceProvider)
        {
            return AddObject(insuranceProvider);
        }

        public Task EditInsuranceProvider(InsuranceProviderSnapshot insuranceProvider)
        {
            return UpdateObject(insuranceProvider);
        }

        public Task<InsuranceProviderSnapshot[]> GetInsuranceProviders(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteInsuranceProvider(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}
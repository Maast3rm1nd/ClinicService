using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class PolicySnapshotRepository : RepositoryBase<PolicySnapshot>, IPolicySnapshotRepository
    {
        public PolicySnapshotRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddPolicy(PolicySnapshot policy)
        {
            return AddObject(policy);
        }

        public Task EditPolicy(PolicySnapshot policy)
        {
            return UpdateObject(policy);
        }

        public async Task<PolicySnapshot?> GetPolicies(Guid[] ids, CancellationToken cancellationToken)
        {
            return (await GetObjectsByIds(ids, cancellationToken)).FirstOrDefault();
        }

        public Task DeletePolicy(Guid policyId)
        {
            return SoftDeleteById(policyId);
        }
    }
}
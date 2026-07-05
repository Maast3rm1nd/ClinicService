using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IPolicySnapshotRepository : IClinicServiceRepository<PolicySnapshot>
    {
        Task AddPolicy(PolicySnapshot policy);

        Task EditPolicy(PolicySnapshot policy);

        Task<PolicySnapshot> GetPolicies(Guid[] ids, CancellationToken cancellationToken);

        Task DeletePolicy(Guid policyId);
    }
}

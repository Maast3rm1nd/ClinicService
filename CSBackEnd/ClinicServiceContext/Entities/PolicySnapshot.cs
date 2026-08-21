using ClinicServiceContext.Enums;

namespace ClinicServiceContext.Entities
{
    public class PolicySnapshot : SnapshotBase
    {
        public string MedicalPolicyNumber { get; set; }

        public PolicyType MedicalPolicyType { get; set; }

        public Guid InsuranceProvider { get; set; }

        public string? Description { get; set; }
    }
}

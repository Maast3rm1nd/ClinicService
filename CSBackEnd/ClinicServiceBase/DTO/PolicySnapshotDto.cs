using ClinicServiceContext.Enums;

namespace ClinicServiceBase.DTO
{
    public class PolicySnapshotDto : SnapshotDtoBase
    {
        public string MedicalPolicyNumber { get; set; }

        public PolicyType? MedicalPolicyType { get; set; }

        public Guid InsuranceProvider { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }
    }
}

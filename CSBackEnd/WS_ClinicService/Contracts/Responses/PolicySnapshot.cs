using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Responses
{
    public class PolicySnapshot
    {
        public Guid Id { get; set; }

        public string MedicalPolicyNumber { get; set; }

        public PolicyType? MedicalPolicyType { get; set; }

        public Guid InsuranceProvider { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }
    }
}
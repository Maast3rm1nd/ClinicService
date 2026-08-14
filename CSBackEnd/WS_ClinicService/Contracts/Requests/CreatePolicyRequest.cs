using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Requests
{
    public class CreatePolicyRequest
    {
        public string MedicalPolicyNumber { get; set; }

        public PolicyType? MedicalPolicyType { get; set; }

        public Guid InsuranceProvider { get; set; }

        public string? Description { get; set; }
    }
}
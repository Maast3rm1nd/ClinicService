using ClinicServiceContext.Enums;

namespace ClinicServiceBase.DTO
{
    public class PolicySnapshotDto
    {
        public Guid Id { get; set; }
        public string MedicalPolicyNumber { get; set; }
        public PolicyType? MedicalPolicyType { get; set; }
        public Guid InsuranceProviderId { get; set; }
        public string? Description { get; set; }

        public InsuranceProviderSnapshotDto InsuranceProvider { get; set; }
        public ICollection<MedicalCardSnapshotDto> MedicalCards { get; set; }
        public ICollection<PatientSnapshotDto> Patients { get; set; }
    }
}

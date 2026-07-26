namespace ClinicServiceBase.DTO
{
    public class InsuranceProviderSnapshotDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsCurrent { get; set; }

        public ICollection<PolicySnapshotDto> Policies { get; set; }
    }
}

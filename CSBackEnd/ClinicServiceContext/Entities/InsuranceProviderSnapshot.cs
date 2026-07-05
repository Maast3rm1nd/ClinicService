namespace ClinicServiceContext.Entities
{
    public class InsuranceProviderSnapshot : SnapshotBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LicenseNumber { get; set; }

        public string? PhoneNumber { get; set; }
    }
}

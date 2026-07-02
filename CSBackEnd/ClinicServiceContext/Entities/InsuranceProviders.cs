namespace ClinicServiceContext.Entities
{
    public class InsuranceProviders : SnapshotBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LicenseNumber { get; set; }

        public string? PhoneNumber { get; set; }
    }
}

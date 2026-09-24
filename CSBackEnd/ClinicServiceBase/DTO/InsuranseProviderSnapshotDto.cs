namespace ClinicServiceBase.DTO
{
    public class InsuranceProviderSnapshotDto : SnapshotDtoBase
    {
        public string Name { get; set; }

        public string LicenseNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }
    }
}

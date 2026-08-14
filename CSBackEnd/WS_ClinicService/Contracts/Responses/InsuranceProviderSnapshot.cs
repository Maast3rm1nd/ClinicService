namespace WS_ClinicService.Contracts.Responses
{
    public class InsuranceProviderSnapshot
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LicenseNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }
    }
}
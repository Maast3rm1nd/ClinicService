namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateInsuranceProviderRequest
    {
        public string? Name { get; set; }

        public string? LicenseNumber { get; set; }

        public string? PhoneNumber { get; set; }
    }
}

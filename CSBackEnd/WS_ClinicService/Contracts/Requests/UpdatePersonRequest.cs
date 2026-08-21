namespace WS_ClinicService.Contracts.Requests
{
    public class UpdatePersonRequest
    {
        public string? FullName { get; set; }

        public string? ShortName { get; set; }

        public string? Login { get; set; }
    }
}

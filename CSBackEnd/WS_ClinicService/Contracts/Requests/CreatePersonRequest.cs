namespace WS_ClinicService.Contracts.Requests
{
    public class CreatePersonRequest
    {
        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}
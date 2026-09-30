namespace WS_ClinicService.Contracts.Requests
{
    public sealed class ValidatePasswordSetupRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public sealed class CompletePasswordSetupRequest
    {
        public string Token { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

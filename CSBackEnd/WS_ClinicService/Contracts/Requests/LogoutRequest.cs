namespace WS_ClinicService.Contracts.Requests
{
    public sealed class LogoutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}

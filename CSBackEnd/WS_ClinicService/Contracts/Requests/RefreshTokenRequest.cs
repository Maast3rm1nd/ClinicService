namespace WS_ClinicService.Contracts.Requests
{
    public sealed class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}

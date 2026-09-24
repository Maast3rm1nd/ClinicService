namespace WS_ClinicService.Contracts.Responses
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

        public int ExpiresIn { get; set; }

        public DateTimeOffset RefreshTokenExpiresAt { get; set; }

        public bool TwoFactorRequired { get; set; }
    }
}
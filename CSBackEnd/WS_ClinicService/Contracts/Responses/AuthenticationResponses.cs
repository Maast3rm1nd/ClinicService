namespace WS_ClinicService.Contracts.Responses
{
    public sealed class PasswordSetupValidationResponse
    {
        public string Login { get; set; } = string.Empty;
    }

    public sealed class TwoFactorSetupResponse
    {
        public string Secret { get; set; } = string.Empty;
    }

    public sealed class AdminTwoFactorResponse
    {
        public bool Enabled { get; set; }

        /// <summary>Shared TOTP secret, returned only at the moment the administrator enables 2FA.</summary>
        public string? Secret { get; set; }
    }

    public sealed class TwoFactorStatusResponse
    {
        public bool Enabled { get; set; }
    }
}

namespace WS_ClinicService.Contracts.Responses
{
    public sealed class CreatedAccountResponse<T>
    {
        public T Account { get; set; } = default!;

        public string SetupToken { get; set; } = string.Empty;

        public DateTimeOffset SetupTokenExpiresAt { get; set; }
    }

    public sealed class PasswordSetupValidationResponse
    {
        public string Login { get; set; } = string.Empty;
    }

    public sealed class TwoFactorSetupResponse
    {
        public string Secret { get; set; } = string.Empty;
    }

    public sealed class TwoFactorStatusResponse
    {
        public bool Enabled { get; set; }
    }
}

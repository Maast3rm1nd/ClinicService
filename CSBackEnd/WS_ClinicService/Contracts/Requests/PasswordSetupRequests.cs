namespace WS_ClinicService.Contracts.Requests
{
    public sealed class ValidatePasswordSetupRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public sealed class ChangePasswordRequest
    {
        public string Login { get; set; } = string.Empty;

        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string? TwoFactorCode { get; set; }
    }

    public sealed class SetDoctorTwoFactorRequest
    {
        public bool Enabled { get; set; }
    }

    public sealed class CompletePasswordSetupRequest
    {
        public string Token { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

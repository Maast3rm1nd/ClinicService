namespace WS_ClinicService.Core.Auth
{
    public sealed class AuthBootstrapOptions
    {
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = "Initial administrator";
    }
}

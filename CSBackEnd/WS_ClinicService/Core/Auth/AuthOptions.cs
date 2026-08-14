namespace WS_ClinicService.Core.Auth
{
    public class AuthOptions
    {
        public List<AuthUser> Users { get; set; } = new();
    }

    public class AuthUser
    {
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
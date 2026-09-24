namespace WS_ClinicService.Contracts.Requests
{
    public sealed class TwoFactorCodeRequest
    {
        public string Code { get; set; } = string.Empty;
    }
}

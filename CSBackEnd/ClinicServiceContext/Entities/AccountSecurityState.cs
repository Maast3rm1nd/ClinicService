namespace ClinicServiceContext.Entities
{
    public sealed class AccountSecurityState
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PersonId { get; set; }

        public int FailedLoginCount { get; set; }

        public DateTimeOffset? LockedUntil { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string? TwoFactorSecret { get; set; }

        public DateTimeOffset? TwoFactorConfirmedAt { get; set; }

        public DateTimeOffset? PasswordChangedAt { get; set; }

        public DateTimeOffset? LastLoginAt { get; set; }

        public DateTimeOffset CreationDateTime { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset EditDateTime { get; set; } = DateTimeOffset.UtcNow;
    }
}

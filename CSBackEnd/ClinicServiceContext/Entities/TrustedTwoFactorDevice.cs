namespace ClinicServiceContext.Entities
{
    public sealed class TrustedTwoFactorDevice
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PersonId { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? LastUsedAt { get; set; }

        public DateTimeOffset? RevokedAt { get; set; }
    }
}

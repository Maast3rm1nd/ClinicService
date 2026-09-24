namespace ClinicServiceContext.Entities
{
    public sealed class RefreshSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PersonId { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? RevokedAt { get; set; }

        public Guid? ReplacedBySessionId { get; set; }

        public string? CreatedByIp { get; set; }

        public string? UserAgent { get; set; }
    }
}

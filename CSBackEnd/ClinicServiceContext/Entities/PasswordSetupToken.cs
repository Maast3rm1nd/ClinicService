namespace ClinicServiceContext.Entities
{
    public sealed class PasswordSetupToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PersonId { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? UsedAt { get; set; }
    }
}

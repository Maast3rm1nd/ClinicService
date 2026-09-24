namespace ClinicServiceContext.Entities
{
    public sealed class SecurityAuditEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? PersonId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public bool Succeeded { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string? Details { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}

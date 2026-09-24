namespace ClinicServiceBase.DTO
{
    public abstract class SnapshotDtoBase
    {
        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        public int Version { get; set; }

        public DateTimeOffset ValidFrom { get; set; }

        public DateTimeOffset? ValidTo { get; set; }
    }
}

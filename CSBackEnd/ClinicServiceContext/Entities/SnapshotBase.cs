namespace ClinicServiceContext.Entities
{
    public class SnapshotBase: BaseEntity, IEditableEntity
    {
        public Guid EntityId { get; set; } = Guid.NewGuid();

        public int Version { get; set; } = 1;

        public DateTimeOffset ValidFrom { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? ValidTo { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsCurrent { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTimeOffset? EditDateTime { get; set; }
    }
}

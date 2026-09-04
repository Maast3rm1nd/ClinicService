namespace ClinicServiceContext.Entities
{
    public class SnapshotBase: BaseEntity, IEditableEntity
    {
        public bool IsCurrent { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTimeOffset? EditDateTime { get; set; }
    }
}

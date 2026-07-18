namespace ClinicServiceContext.Entities
{
    public class SnapshotBase: BaseEntity, IEditableEntity
    {
        public bool IsCurrent { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }
    }
}

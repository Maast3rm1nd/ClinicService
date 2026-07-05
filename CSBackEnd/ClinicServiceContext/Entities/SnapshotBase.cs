namespace ClinicServiceContext.Entities
{
    public class SnapshotBase: BaseEntity
    {
        public bool IsCurrent { get; set; }

        public bool IsDeleted { get; set; }
    }
}

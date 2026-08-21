namespace ClinicServiceContext.Entities
{
    public class MedicalCardSnapshot : SnapshotBase
    {
        public Guid Patient { get; set; }

        public ulong RecordNumber { get; set; }

        public Guid? Policy { get; set; }

        public ICollection<Guid>? Diagnoses { get; set; }
    }
}

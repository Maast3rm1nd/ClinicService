namespace ClinicServiceBase.DTO
{
    public class MedicalCardSnapshotDto : SnapshotDtoBase
    {
        public Guid Patient { get; set; }

        public ulong RecordNumber { get; set; }

        public Guid? Policy { get; set; }

        public List<Guid>? Diagnoses { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }
    }
}

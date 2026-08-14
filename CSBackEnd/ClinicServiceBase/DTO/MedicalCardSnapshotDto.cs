namespace ClinicServiceBase.DTO
{
    public class MedicalCardSnapshotDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public ulong RecordNumber { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
        public Guid PolicyId { get; set; }

        public PatientSnapshotDto Patient { get; set; }
        public PolicySnapshotDto Policy { get; set; }
        public ICollection<DiagnosisSnapshotDto> Diagnoses { get; set; }
        public ICollection<AppointmentSnapshotDto> Appointments { get; set; }
    }
}

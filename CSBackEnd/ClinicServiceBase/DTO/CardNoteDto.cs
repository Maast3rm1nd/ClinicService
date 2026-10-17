namespace ClinicServiceBase.DTO
{
    public class CardNoteDto : SnapshotDtoBase
    {
        public Guid MedicalCard { get; set; }

        public Guid Doctor { get; set; }

        public Guid? Appointment { get; set; }

        public DateTimeOffset DateNote { get; set; }

        public string? PatientsComplaints { get; set; }

        public List<Guid>? Diagnoses { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }
    }
}

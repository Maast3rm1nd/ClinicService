namespace ClinicServiceContext.Entities
{
    public class CardNote : SnapshotBase
    {
        public Guid MedicalCard { get; set; }

        public Guid Doctor { get; set; }

        public Guid? Appointment { get; set; }

        public DateTimeOffset DateNote { get; set; }

        public string? PatientsComplaints { get; set; }

        public ICollection<Guid>? Diagnoses { get; set; }
    }
}

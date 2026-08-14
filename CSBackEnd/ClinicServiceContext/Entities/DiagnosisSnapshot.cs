namespace ClinicServiceContext.Entities
{
    public class DiagnosisSnapshot : SnapshotBase
    {
        public string IcdCode { get; set; }

        public Guid Doctor { get; set; }

        public Guid? EditedDoctor { get; set; }

        public Guid MedicalCard { get; set; }

        public DiagnosisStatus Status { get; set; }
    }
}

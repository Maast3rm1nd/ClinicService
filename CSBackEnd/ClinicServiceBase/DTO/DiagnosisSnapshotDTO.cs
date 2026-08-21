using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DTO
{
    public class DiagnosisSnapshotDto
    {
        public Guid Id { get; set; }

        public string IcdCode { get; set; }

        public Guid Doctor { get; set; }

        public Guid? EditedDoctor { get; set; }

        public Guid MedicalCard { get; set; }

        public DiagnosisStatus Status { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }
    }
}

using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DTO
{
    public class DiagnosisSnapshotDto
    {
        public Guid Id { get; set; }
        public string IcdCode { get; set; }
        public Guid DoctorId { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
        public Guid EditedDoctorId { get; set; }
        public DateTimeOffset? EditDateTime { get; set; }
        public Guid MedicalCardId { get; set; }
        public DiagnosisStatus Status { get; set; }

        public DoctorsDto Doctor { get; set; }
        public DoctorsDto EditedDoctor { get; set; }
        public MedicalCardSnapshotDto MedicalCard { get; set; }
    }

}

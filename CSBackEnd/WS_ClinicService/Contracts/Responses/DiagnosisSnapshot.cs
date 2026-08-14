using ClinicServiceContext.Entities;

namespace WS_ClinicService.Contracts.Responses
{
    public class DiagnosisSnapshot
    {
        public Guid Id { get; set; }

        public string IcdCode { get; set; }

        public Guid Doctor { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }

        public Guid? EditedDoctor { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }

        public Guid MedicalCard { get; set; }

        public DiagnosisStatus Status { get; set; }
    }
}
using ClinicServiceContext.Entities;

namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateDiagnosisRequest
    {
        public string? IcdCode { get; set; }

        public Guid? Doctor { get; set; }

        public Guid? EditedDoctor { get; set; }

        public Guid? MedicalCard { get; set; }

        public DiagnosisStatus? Status { get; set; }
    }
}

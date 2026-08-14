using ClinicServiceContext.Entities;

namespace WS_ClinicService.Contracts.Requests
{
    public class CreateDiagnosisRequest
    {
        public string IcdCode { get; set; }

        public Guid Doctor { get; set; }

        public Guid MedicalCard { get; set; }

        public DiagnosisStatus? Status { get; set; }
    }
}
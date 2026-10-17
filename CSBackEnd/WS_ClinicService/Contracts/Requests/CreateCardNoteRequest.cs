using ClinicServiceContext.Entities;

namespace WS_ClinicService.Contracts.Requests
{
    public class CreateCardNoteRequest
    {
        public Guid MedicalCard { get; set; }

        public Guid Doctor { get; set; }

        public Guid? Appointment { get; set; }

        public DateTimeOffset DateNote { get; set; }

        public string? PatientsComplaints { get; set; }

        /// <summary>Entity ids of diagnoses that already exist in the medical card.</summary>
        public List<Guid>? Diagnoses { get; set; }

        /// <summary>Diagnoses to create in the medical card together with the note.</summary>
        public List<NewCardNoteDiagnosis>? NewDiagnoses { get; set; }
    }

    public class NewCardNoteDiagnosis
    {
        public string IcdCode { get; set; } = string.Empty;

        public string? Name { get; set; }

        public DiagnosisStatus? Status { get; set; }
    }
}

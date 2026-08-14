namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateMedicalCardRequest
    {
        public Guid? Patient { get; set; }

        public Guid? Policy { get; set; }

        public List<Guid>? Diagnoses { get; set; }
    }
}

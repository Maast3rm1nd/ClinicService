namespace WS_ClinicService.Contracts.Requests
{
    public class CreateMedicalCardRequest
    {
        public Guid Patient { get; set; }

        public Guid Policy { get; set; }

        public List<Guid>? Diagnoses { get; set; }
    }
}
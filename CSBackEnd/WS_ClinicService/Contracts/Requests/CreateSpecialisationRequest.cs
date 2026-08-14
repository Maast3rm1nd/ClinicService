namespace WS_ClinicService.Contracts.Requests
{
    public class CreateSpecialisationRequest
    {
        public string Name { get; set; }

        public List<Guid>? Doctors { get; set; }
    }
}
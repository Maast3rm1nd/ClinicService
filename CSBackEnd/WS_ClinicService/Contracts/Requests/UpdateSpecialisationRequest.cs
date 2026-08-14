namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateSpecialisationRequest
    {
        public string? Name { get; set; }

        public List<Guid>? Doctors { get; set; }
    }
}

namespace WS_ClinicService.Contracts.Responses
{
    public class SpecialisationSnapshot
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Guid>? Doctors { get; set; }
    }
}
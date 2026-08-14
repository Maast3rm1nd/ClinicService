namespace WS_ClinicService.Contracts.Responses
{
    public class Schedule
    {
        public Guid Id { get; set; }

        public Guid Doctor { get; set; }

        public List<Guid> Appointments { get; set; }
    }
}
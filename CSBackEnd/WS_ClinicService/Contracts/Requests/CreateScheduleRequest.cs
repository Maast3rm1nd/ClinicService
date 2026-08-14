namespace WS_ClinicService.Contracts.Requests
{
    public class CreateScheduleRequest
    {
        public Guid Doctor { get; set; }

        public List<Guid>? Appointments { get; set; }
    }
}
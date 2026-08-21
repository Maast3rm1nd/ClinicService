namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateScheduleRequest
    {
        public Guid? Doctor { get; set; }

        public List<Guid>? Appointments { get; set; }
    }
}

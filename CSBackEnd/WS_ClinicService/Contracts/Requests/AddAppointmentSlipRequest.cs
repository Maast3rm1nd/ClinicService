namespace WS_ClinicService.Contracts.Requests
{
    public class AddAppointmentSlipRequest
    {
        public Guid ScheduleId { get; set; }

        public DateTimeOffset AppointmentDateTime { get; set; }
    }
}
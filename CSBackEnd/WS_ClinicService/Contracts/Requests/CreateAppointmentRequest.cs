namespace WS_ClinicService.Contracts.Requests
{
    public class CreateAppointmentRequest
    {
        public Guid Patient { get; set; }

        public Guid MedicalCard { get; set; }

        public Guid Doctor { get; set; }

        public DateTimeOffset AppointmentDateTime { get; set; }

        public Guid CreatedBy { get; set; }

        public string? PreliminaryReason { get; set; }
    }
}
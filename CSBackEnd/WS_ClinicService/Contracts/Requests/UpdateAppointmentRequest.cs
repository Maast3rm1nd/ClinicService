using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateAppointmentRequest
    {
        public Guid EditedBy { get; set; }

        public DateTimeOffset? AppointmentDateTime { get; set; }

        public AppointmentStatus? Status { get; set; }

        public string? PreliminaryReason { get; set; }
    }
}
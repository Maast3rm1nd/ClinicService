using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Responses
{
    public class AppointmentSnapshot
    {
        public Guid Id { get; set; }

        public Guid Patient { get; set; }

        public Guid MedicalCard { get; set; }

        public Guid Doctor { get; set; }

        public DateTimeOffset AppointmentDateTime { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }

        public Guid? EditedBy { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public string? PreliminaryReason { get; set; }
    }
}
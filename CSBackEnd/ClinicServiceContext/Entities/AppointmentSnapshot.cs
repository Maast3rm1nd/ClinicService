using ClinicServiceContext.Enums;

namespace ClinicServiceContext.Entities
{
    public class AppointmentSnapshot : SnapshotBase
    {

        public Guid Id { get; set; }

        public Guid Patient { get; set; }

        public Guid MedicalCard { get; set; }

        public Guid Doctor {  get; set; }

        public DateTimeOffset AppointmentDateTime { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? EditedBy { get; set; }

        public AppointmentStatus Status { get; set; }

        public string? PremilinaryReason { get; set; }
    }
}

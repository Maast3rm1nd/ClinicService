using ClinicServiceContext.Enums;

namespace ClinicServiceBase.DTO
{
    public class AppointmentSnapshotDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MedicalCardId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTimeOffset AppointmentDateTime { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
        public Guid? EditedBy { get; set; }
        public DateTimeOffset? EditDateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? PreliminaryReason { get; set; }
        public bool IsCurrent { get; set; }

        // Навигационные свойства
        public PatientSnapshotDto Patient { get; set; }
        public MedicalCardSnapshotDto MedicalCard { get; set; }
        public DoctorsDto Doctor { get; set; }
        public AdministratorsDto CreatedByAdmin { get; set; }
        public AdministratorsDto EditedByAdmin { get; set; }
        public ScheduleDto Schedule { get; set; }
    }
}

namespace ClinicServiceBase.DTO
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DoctorsDto Doctor { get; set; }
        public ICollection<AppointmentSnapshotDto> Appointments { get; set; }
    }
}

namespace ClinicServiceBase.DTO
{
    public class AdministratorsDto
    {
        public Guid Id { get; set; }

        public PersonSnapshotDto Person { get; set; }
        public ICollection<AppointmentSnapshotDto> CreatedAppointments { get; set; }
        public ICollection<AppointmentSnapshotDto> EditedAppointments { get; set; }
    }
}

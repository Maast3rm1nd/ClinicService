namespace ClinicServiceBase.DTO
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }

        public Guid Doctor { get; set; }

        public List<Guid> Appointments { get; set; }
    }
}

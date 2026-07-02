namespace ClinicServiceContext.Entities
{
    public class Schedule
    {
        public Guid Id { get; set; }

        public Guid Doctor {  get; set; }

        public ICollection<Guid> Appointments { get; set; }
    }
}

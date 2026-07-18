namespace ClinicServiceContext.Entities
{
    public class Schedule: IEditableEntity
    {
        public Guid Id { get; set; }

        public Guid Doctor {  get; set; }

        public ICollection<Guid> Appointments { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }
    }
}

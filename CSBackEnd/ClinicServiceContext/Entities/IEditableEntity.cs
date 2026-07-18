namespace ClinicServiceContext.Entities
{
    public interface IEditableEntity
    {
        DateTimeOffset? EditDateTime { get; set; }
    }
}

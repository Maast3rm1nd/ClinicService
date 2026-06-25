namespace ClinicServiceContext.Entities
{
    public class Entity
    { 
        public Guid Id { get; set; } = new Guid();
    }

    public class BaseEntity: Entity
    {
        public DateTimeOffset CreationDateTime { get; set; } = DateTimeOffset.UtcNow;
    }
}

namespace ClinicServiceContext.Entities
{
    public class Entity
    { 
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class BaseEntity: Entity
    {
        public DateTimeOffset CreationDateTime { get; set; } = DateTimeOffset.UtcNow;
    }
}

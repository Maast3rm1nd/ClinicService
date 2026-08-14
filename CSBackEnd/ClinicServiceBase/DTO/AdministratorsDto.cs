namespace ClinicServiceBase.DTO
{
    public class AdministratorsDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string Login { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }
    }
}

using ClinicServiceContext.Enums;

namespace ClinicServiceBase.DTO
{
    public class PersonSnapshotDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? ShortName { get; set; }
        public string Login { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
        public DateTimeOffset? EditDateTime { get; set; }
        public PersonType PersonType { get; set; }
        public DoctorsDto Doctor { get; set; }
        public AdministratorsDto Administrator { get; set; }
    }
}

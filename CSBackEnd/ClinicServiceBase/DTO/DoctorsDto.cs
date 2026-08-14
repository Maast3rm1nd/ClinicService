using ClinicServiceContext.Enums;

namespace ClinicServiceBase.DTO
{
    public class DoctorsDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string Login { get; set; }

        public List<Guid> Specialisations { get; set; }

        public DoctorWorkStatus DoctorWorkStatus { get; set; }
    }
}

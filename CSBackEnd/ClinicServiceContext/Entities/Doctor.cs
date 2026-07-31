using ClinicServiceContext.Enums;

namespace ClinicServiceContext.Entities
{
    public class Doctor: PersonSnapshot
    {
        public List<Guid> Specialisations { get; set; }

        public DoctorWorkStatus DoctorWorkStatus { get; set; }
    }
}

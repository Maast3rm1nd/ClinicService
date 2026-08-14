using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Responses
{
    public class Doctors
    {
        public Guid Id { get; set; }

        public List<Guid> Specialisations { get; set; }

        public DoctorWorkStatus DoctorWorkStatus { get; set; }
    }
}
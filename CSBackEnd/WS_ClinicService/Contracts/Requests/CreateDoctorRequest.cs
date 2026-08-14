using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Requests
{
    public class CreateDoctorRequest
    {
        public Guid Id { get; set; }

        public List<Guid> Specialisations { get; set; }

        public DoctorWorkStatus DoctorWorkStatus { get; set; }
    }
}
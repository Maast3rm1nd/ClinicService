using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateDoctorRequest
    {
        public string? FullName { get; set; }

        public string? ShortName { get; set; }

        public string? Login { get; set; }

        public List<Guid>? Specialisations { get; set; }

        public DoctorWorkStatus? DoctorWorkStatus { get; set; }
    }
}

using ClinicServiceContext.Enums;

namespace WS_ClinicService.Contracts.Requests
{
    public class CreateDoctorRequest
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string Login { get; set; }

        public List<Guid> Specialisations { get; set; }

        public EmployeeWorkStatus EmployeeWorkStatus { get; set; }
    }
}

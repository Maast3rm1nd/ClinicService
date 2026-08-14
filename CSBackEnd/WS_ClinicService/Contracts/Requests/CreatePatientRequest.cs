namespace WS_ClinicService.Contracts.Requests
{
    public class CreatePatientRequest
    {
        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string? PassportNumber { get; set; }

        public DateTimeOffset BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

        public string? BloodGroup { get; set; }

        public string? Allergies { get; set; }
    }
}
namespace WS_ClinicService.Contracts.Responses
{
    public class PatientSnapshot
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string? PassportNumber { get; set; }

        public DateTimeOffset BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

        public string? BloodGroup { get; set; }

        public string? Allergies { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }

        public DateTimeOffset? EditDateTime { get; set; }
    }
}
namespace WS_ClinicService.Contracts.Responses
{
    public class MedicalCardSnapshot
    {
        public Guid Id { get; set; }

        public Guid Patient { get; set; }

        public ulong RecordNumber { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }

        public Guid Policy { get; set; }

        public List<Guid>? Diagnoses { get; set; }
    }
}
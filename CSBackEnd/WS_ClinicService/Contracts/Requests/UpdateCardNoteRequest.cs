namespace WS_ClinicService.Contracts.Requests
{
    public class UpdateCardNoteRequest
    {
        public DateTimeOffset? DateNote { get; set; }

        public string? PatientsComplaints { get; set; }

        public Guid? EditedBy { get; set; }

        /// <summary>Entity ids of the diagnoses the note refers to; replaces the current list.</summary>
        public List<Guid>? Diagnoses { get; set; }
    }
}

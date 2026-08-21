namespace ClinicServiceBase.DTO
{
    public class SpecialisationSnapshotDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Guid>? Doctors { get; set; }
    }
}

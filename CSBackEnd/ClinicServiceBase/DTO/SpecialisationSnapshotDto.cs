namespace ClinicServiceBase.DTO
{
    public class SpecialisationSnapshotDto : SnapshotDtoBase
    {
        public string Name { get; set; }

        public List<Guid>? Doctors { get; set; }
    }
}

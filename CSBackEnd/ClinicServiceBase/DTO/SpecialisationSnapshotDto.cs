namespace ClinicServiceBase.DTO
{
    public class SpecialisationSnapshotDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsCurrent { get; set; }

        public ICollection<DoctorsDto> Doctors { get; set; }

    }
}

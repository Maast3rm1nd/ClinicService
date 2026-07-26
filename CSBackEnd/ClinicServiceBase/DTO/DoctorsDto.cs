using ClinicServiceContext.Enums;

namespace ClinicServiceBase.DTO
{
    public class DoctorsDto
    {
        public Guid Id { get; set; }
        public DoctorWorkStatus DoctorWorkStatus { get; set; }

        public PersonSnapshotDto Person { get; set; }
        public ICollection<SpecialisationSnapshotDto> Specialisations { get; set; }
        public ICollection<ScheduleDto> Schedules { get; set; }
        public ICollection<AppointmentSnapshotDto> Appointments { get; set; }
        public ICollection<DiagnosisSnapshotDto> Diagnoses { get; set; }
        public ICollection<DiagnosisSnapshotDto> EditedDiagnoses { get; set; }
    }
}

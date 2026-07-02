using System.Diagnostics;

namespace ClinicServiceContext.Entities
{
    public class SpecialisationSnapshot : SnapshotBase
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<Guid>? Doctors { get; set; }
    }
}

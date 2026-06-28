using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicServiceContext.Entities
{
    public class PatientSnapshot: SnapshotBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string PassportNumber { get; set; }

        public DateTimeOffset BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

        public string? BloodGroup { get; set; }

        public string? Allergies { get; set; }

    }
}

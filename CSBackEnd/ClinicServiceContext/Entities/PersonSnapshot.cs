using ClinicServiceContext.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicServiceContext.Entities
{
    public class PersonSnapshot: SnapshotBase
    {
        public string FullName { get; set; }

        public string? ShortName { get; set; }

        public string Login { get; set; }

        public PersonType Type { get; set; }
    }
}

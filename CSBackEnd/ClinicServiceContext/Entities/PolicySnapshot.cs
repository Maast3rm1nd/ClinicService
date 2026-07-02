using ClinicServiceContext.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicServiceContext.Entities
{
    public class PolicySnapshot : SnapshotBase
    {
        public Guid Id { get; set; }

        public string MedicalPolicyNumber { get; set; }

        public PolicyType MedicalPolicyType { get; set; }

        public Guid InsuranceProvider { get; set; }

        public string? Description { get; set; }
    }
}

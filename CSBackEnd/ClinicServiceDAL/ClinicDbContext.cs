using ClinicServiceContext.Entities;
using ClinicServiceContext.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicServiceDAL
{
    public class ClinicDbContext : DbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
        {
        }

        public DbSet<PatientSnapshot> PatientSnapshots { get; set; }

        public DbSet<MedicalCardSnapshot> MedicalCardSnapshots { get; set; }

        public DbSet<PolicySnapshot> PolicySnapshots { get; set; }

        public DbSet<InsuranceProviderSnapshot> InsuranceProviderSnapshots { get; set; }

        public DbSet<SpecialisationSnapshot> SpecialisationSnapshots { get; set; }

        public DbSet<DiagnosisSnapshot> DiagnosisSnapshots { get; set; }

        public DbSet<AppointmentSnapshot> AppointmentSnapshots { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<PersonSnapshot> PersonSnapshots { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Administrator> Administrators { get; set; }

        public DbSet<AccountSecurityState> AccountSecurityStates { get; set; }

        public DbSet<RefreshSession> RefreshSessions { get; set; }

        public DbSet<SecurityAuditEvent> SecurityAuditEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureSnapshot(modelBuilder.Entity<AppointmentSnapshot>());
            ConfigureSnapshot(modelBuilder.Entity<DiagnosisSnapshot>());
            ConfigureSnapshot(modelBuilder.Entity<InsuranceProviderSnapshot>());
            ConfigureSnapshot(modelBuilder.Entity<MedicalCardSnapshot>());
            ConfigureSnapshot(modelBuilder.Entity<PatientSnapshot>());
            ConfigureSnapshot(modelBuilder.Entity<PolicySnapshot>());
            ConfigureSnapshot(modelBuilder.Entity<SpecialisationSnapshot>());

            modelBuilder.Entity<MedicalCardSnapshot>()
                .Property(e => e.RecordNumber)
                .HasConversion(
                    v => (decimal)v,
                    v => (ulong)v);

            modelBuilder.Entity<MedicalCardSnapshot>().PrimitiveCollection(e => e.Diagnoses);
            modelBuilder.Entity<SpecialisationSnapshot>().PrimitiveCollection(e => e.Doctors);
            modelBuilder.Entity<Schedule>().PrimitiveCollection(e => e.Appointments);
            modelBuilder.Entity<Doctor>().PrimitiveCollection(e => e.Specialisations);

            modelBuilder.Entity<PersonSnapshot>()
                .HasDiscriminator<PersonType>(p => p.Type)
                .HasValue<PersonSnapshot>(PersonType.Person)
                .HasValue<Doctor>(PersonType.Doctor)
                .HasValue<Administrator>(PersonType.Administrator);

            ConfigureSnapshot(modelBuilder.Entity<PersonSnapshot>());

            modelBuilder.Entity<AccountSecurityState>()
                .HasIndex(state => state.PersonId)
                .IsUnique();

            modelBuilder.Entity<RefreshSession>()
                .HasIndex(session => session.TokenHash)
                .IsUnique();

            modelBuilder.Entity<RefreshSession>()
                .HasIndex(session => new { session.PersonId, session.RevokedAt });

            modelBuilder.Entity<SecurityAuditEvent>()
                .HasIndex(auditEvent => new { auditEvent.PersonId, auditEvent.CreatedAt });
        }

        private static void ConfigureSnapshot<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity)
            where TEntity : SnapshotBase
        {
            entity.Property(snapshot => snapshot.Version)
                .IsConcurrencyToken();

            entity.HasIndex(snapshot => new { snapshot.EntityId, snapshot.Version })
                .IsUnique();

            entity.HasIndex(snapshot => new { snapshot.EntityId, snapshot.IsCurrent });

            entity.HasQueryFilter(snapshot => snapshot.IsCurrent && !snapshot.IsDeleted);
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicServiceDAL.Migrations.Mssql
{
    /// <inheritdoc />
    public partial class BackfillSnapshotVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in new[]
            {
                "AppointmentSnapshots", "DiagnosisSnapshots", "InsuranceProviderSnapshots",
                "MedicalCardSnapshots", "PatientSnapshots", "PersonSnapshots",
                "PolicySnapshots", "SpecialisationSnapshots"
            })
            {
                migrationBuilder.Sql($"UPDATE [{table}] SET [EntityId] = [Id], [Version] = 1, [ValidFrom] = [CreationDateTime] WHERE [Version] = 0;");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

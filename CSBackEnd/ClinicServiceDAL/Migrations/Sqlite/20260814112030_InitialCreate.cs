using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicServiceDAL.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Patient = table.Column<Guid>(type: "TEXT", nullable: false),
                    MedicalCard = table.Column<Guid>(type: "TEXT", nullable: false),
                    Doctor = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppointmentDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    EditedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PremilinaryReason = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IcdCode = table.Column<string>(type: "TEXT", nullable: false),
                    Doctor = table.Column<Guid>(type: "TEXT", nullable: false),
                    EditedDoctor = table.Column<Guid>(type: "TEXT", nullable: true),
                    MedicalCard = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceProviderSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceProviderSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalCardSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Patient = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecordNumber = table.Column<decimal>(type: "TEXT", nullable: false),
                    Policy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Diagnoses = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalCardSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    ShortName = table.Column<string>(type: "TEXT", nullable: true),
                    PassportNumber = table.Column<string>(type: "TEXT", nullable: false),
                    BirthDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    BloodGroup = table.Column<string>(type: "TEXT", nullable: true),
                    Allergies = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    ShortName = table.Column<string>(type: "TEXT", nullable: true),
                    Login = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<byte>(type: "INTEGER", nullable: false),
                    Specialisations = table.Column<string>(type: "TEXT", nullable: true),
                    DoctorWorkStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicySnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MedicalPolicyNumber = table.Column<string>(type: "TEXT", nullable: false),
                    MedicalPolicyType = table.Column<int>(type: "INTEGER", nullable: false),
                    InsuranceProvider = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicySnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Doctor = table.Column<Guid>(type: "TEXT", nullable: false),
                    Appointments = table.Column<string>(type: "TEXT", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialisationSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Doctors = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialisationSnapshots", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentSnapshots");

            migrationBuilder.DropTable(
                name: "DiagnosisSnapshots");

            migrationBuilder.DropTable(
                name: "InsuranceProviderSnapshots");

            migrationBuilder.DropTable(
                name: "MedicalCardSnapshots");

            migrationBuilder.DropTable(
                name: "PatientSnapshots");

            migrationBuilder.DropTable(
                name: "PersonSnapshots");

            migrationBuilder.DropTable(
                name: "PolicySnapshots");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "SpecialisationSnapshots");
        }
    }
}

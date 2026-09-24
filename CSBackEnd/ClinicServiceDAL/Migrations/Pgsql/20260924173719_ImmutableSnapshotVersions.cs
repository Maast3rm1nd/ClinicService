using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicServiceDAL.Migrations.Pgsql
{
    /// <inheritdoc />
    public partial class ImmutableSnapshotVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "SpecialisationSnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "SpecialisationSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "SpecialisationSnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "SpecialisationSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "SpecialisationSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "PolicySnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "PolicySnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "PolicySnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "PolicySnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PolicySnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "PersonSnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "PersonSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "PersonSnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "PersonSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PersonSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "PatientSnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "PatientSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "PatientSnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "PatientSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PatientSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "MedicalCardSnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "MedicalCardSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "MedicalCardSnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "MedicalCardSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "MedicalCardSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "InsuranceProviderSnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "InsuranceProviderSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "InsuranceProviderSnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "InsuranceProviderSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "InsuranceProviderSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "DiagnosisSnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "DiagnosisSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "DiagnosisSnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "DiagnosisSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "DiagnosisSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "AppointmentSnapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "AppointmentSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "AppointmentSnapshots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "AppointmentSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AppointmentSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SpecialisationSnapshots_EntityId_IsCurrent",
                table: "SpecialisationSnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialisationSnapshots_EntityId_Version",
                table: "SpecialisationSnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicySnapshots_EntityId_IsCurrent",
                table: "PolicySnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_PolicySnapshots_EntityId_Version",
                table: "PolicySnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonSnapshots_EntityId_IsCurrent",
                table: "PersonSnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonSnapshots_EntityId_Version",
                table: "PersonSnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientSnapshots_EntityId_IsCurrent",
                table: "PatientSnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientSnapshots_EntityId_Version",
                table: "PatientSnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalCardSnapshots_EntityId_IsCurrent",
                table: "MedicalCardSnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalCardSnapshots_EntityId_Version",
                table: "MedicalCardSnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceProviderSnapshots_EntityId_IsCurrent",
                table: "InsuranceProviderSnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceProviderSnapshots_EntityId_Version",
                table: "InsuranceProviderSnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisSnapshots_EntityId_IsCurrent",
                table: "DiagnosisSnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisSnapshots_EntityId_Version",
                table: "DiagnosisSnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSnapshots_EntityId_IsCurrent",
                table: "AppointmentSnapshots",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSnapshots_EntityId_Version",
                table: "AppointmentSnapshots",
                columns: new[] { "EntityId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecialisationSnapshots_EntityId_IsCurrent",
                table: "SpecialisationSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_SpecialisationSnapshots_EntityId_Version",
                table: "SpecialisationSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_PolicySnapshots_EntityId_IsCurrent",
                table: "PolicySnapshots");

            migrationBuilder.DropIndex(
                name: "IX_PolicySnapshots_EntityId_Version",
                table: "PolicySnapshots");

            migrationBuilder.DropIndex(
                name: "IX_PersonSnapshots_EntityId_IsCurrent",
                table: "PersonSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_PersonSnapshots_EntityId_Version",
                table: "PersonSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_PatientSnapshots_EntityId_IsCurrent",
                table: "PatientSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_PatientSnapshots_EntityId_Version",
                table: "PatientSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_MedicalCardSnapshots_EntityId_IsCurrent",
                table: "MedicalCardSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_MedicalCardSnapshots_EntityId_Version",
                table: "MedicalCardSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceProviderSnapshots_EntityId_IsCurrent",
                table: "InsuranceProviderSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceProviderSnapshots_EntityId_Version",
                table: "InsuranceProviderSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_DiagnosisSnapshots_EntityId_IsCurrent",
                table: "DiagnosisSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_DiagnosisSnapshots_EntityId_Version",
                table: "DiagnosisSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentSnapshots_EntityId_IsCurrent",
                table: "AppointmentSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentSnapshots_EntityId_Version",
                table: "AppointmentSnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "SpecialisationSnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "SpecialisationSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "SpecialisationSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "SpecialisationSnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SpecialisationSnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "PolicySnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "PolicySnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "PolicySnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "PolicySnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PolicySnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "PersonSnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "PersonSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "PersonSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "PersonSnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PersonSnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "PatientSnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "PatientSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "PatientSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "PatientSnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PatientSnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "MedicalCardSnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "MedicalCardSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "MedicalCardSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "MedicalCardSnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "MedicalCardSnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "InsuranceProviderSnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "InsuranceProviderSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "InsuranceProviderSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "InsuranceProviderSnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "InsuranceProviderSnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "DiagnosisSnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "DiagnosisSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "DiagnosisSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "DiagnosisSnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "DiagnosisSnapshots");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "AppointmentSnapshots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AppointmentSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "AppointmentSnapshots");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "AppointmentSnapshots");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AppointmentSnapshots");
        }
    }
}

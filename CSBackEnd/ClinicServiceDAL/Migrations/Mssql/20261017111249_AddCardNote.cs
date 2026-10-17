using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicServiceDAL.Migrations.Mssql
{
    /// <inheritdoc />
    public partial class AddCardNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalCard = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Doctor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Appointment = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateNote = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PatientsComplaints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diagnoses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ValidTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardNotes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardNotes_EntityId_IsCurrent",
                table: "CardNotes",
                columns: new[] { "EntityId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_CardNotes_EntityId_Version",
                table: "CardNotes",
                columns: new[] { "EntityId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardNotes");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicServiceDAL.Migrations.Sqlite
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MedicalCard = table.Column<Guid>(type: "TEXT", nullable: false),
                    Doctor = table.Column<Guid>(type: "TEXT", nullable: false),
                    Appointment = table.Column<Guid>(type: "TEXT", nullable: true),
                    DateNote = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    PatientsComplaints = table.Column<string>(type: "TEXT", nullable: true),
                    Diagnoses = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ValidTo = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ChangedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    EditDateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
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

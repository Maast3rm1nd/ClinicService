using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicServiceDAL.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RenameDoctorWorkStatusToEmployeeWorkStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DoctorWorkStatus",
                table: "PersonSnapshots",
                newName: "EmployeeWorkStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeWorkStatus",
                table: "PersonSnapshots",
                newName: "DoctorWorkStatus");
        }
    }
}

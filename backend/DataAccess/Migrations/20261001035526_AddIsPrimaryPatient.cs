using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseClinic.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPrimaryPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Patients",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Patients");
        }
    }
}

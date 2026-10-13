using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseClinic.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToDepartmentAndDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Doctors",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Departments",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_Slug",
                table: "Doctors",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Slug",
                table: "Departments",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doctors_Slug",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Slug",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Departments");
        }
    }
}

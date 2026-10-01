using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseClinic.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RefreshTokenExpiryTime",
                table: "Accounts",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenHash",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                table: "Accounts");
        }
    }
}

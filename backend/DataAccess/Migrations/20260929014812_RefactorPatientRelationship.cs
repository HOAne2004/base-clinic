using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseClinic.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPatientRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Patients_PrimaryPatientId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PrimaryPatientId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PrimaryPatientId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RelationshipType",
                table: "Patients");

            migrationBuilder.CreateTable(
                name: "PatientDelegations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObserverAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationshipType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDelegations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientDelegations_ObserverAccountId",
                table: "PatientDelegations",
                column: "ObserverAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDelegations_TargetPatientId_ObserverAccountId",
                table: "PatientDelegations",
                columns: new[] { "TargetPatientId", "ObserverAccountId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientDelegations");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryPatientId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelationshipType",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PrimaryPatientId",
                table: "Patients",
                column: "PrimaryPatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Patients_PrimaryPatientId",
                table: "Patients",
                column: "PrimaryPatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

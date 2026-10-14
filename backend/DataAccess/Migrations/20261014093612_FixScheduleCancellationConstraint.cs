using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseClinic.Migrations
{
    /// <inheritdoc />
    public partial class FixScheduleCancellationConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleCancellationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleCancellationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleCancellationRequests_Accounts_RequestedByAccountId",
                        column: x => x.RequestedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleCancellationRequests_Accounts_ReviewedByAccountId",
                        column: x => x.ReviewedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScheduleCancellationRequests_DoctorSchedules_DoctorScheduleId",
                        column: x => x.DoctorScheduleId,
                        principalTable: "DoctorSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleCancellationRequests_DoctorScheduleId",
                table: "ScheduleCancellationRequests",
                column: "DoctorScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleCancellationRequests_RequestedByAccountId",
                table: "ScheduleCancellationRequests",
                column: "RequestedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleCancellationRequests_ReviewedByAccountId",
                table: "ScheduleCancellationRequests",
                column: "ReviewedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleCancellationRequests_Status",
                table: "ScheduleCancellationRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleCancellationRequests");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseClinic.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDoctorSchedule_AddSlotItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorSchedules_DoctorId_WorkDate_StartTime",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "OnlineBookingCapacity",
                table: "DoctorSchedules");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "DoctorSchedules",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "DoctorSchedules",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "DoctorSchedules",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancelledAt",
                table: "DoctorSchedules",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledBy",
                table: "DoctorSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "DoctorSchedules",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnlineBookingEnabled",
                table: "DoctorSchedules",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnlineBookingEnabled",
                table: "Departments",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorScheduleItemId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DoctorScheduleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    BookedCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorScheduleItems_DoctorSchedules_DoctorScheduleId",
                        column: x => x.DoctorScheduleId,
                        principalTable: "DoctorSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_Status",
                table: "DoctorSchedules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorScheduleItemId",
                table: "Appointments",
                column: "DoctorScheduleItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorScheduleItems_DoctorScheduleId_StartTime",
                table: "DoctorScheduleItems",
                columns: new[] { "DoctorScheduleId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorScheduleItems_Status",
                table: "DoctorScheduleItems",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorScheduleItems_DoctorScheduleItemId",
                table: "Appointments",
                column: "DoctorScheduleItemId",
                principalTable: "DoctorScheduleItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorScheduleItems_DoctorScheduleItemId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "DoctorScheduleItems");

            migrationBuilder.DropIndex(
                name: "IX_DoctorSchedules_Status",
                table: "DoctorSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorScheduleItemId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "OnlineBookingEnabled",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "OnlineBookingEnabled",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DoctorScheduleItemId",
                table: "Appointments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "DoctorSchedules",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "DoctorSchedules",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<int>(
                name: "OnlineBookingCapacity",
                table: "DoctorSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId_WorkDate_StartTime",
                table: "DoctorSchedules",
                columns: new[] { "DoctorId", "WorkDate", "StartTime" });
        }
    }
}

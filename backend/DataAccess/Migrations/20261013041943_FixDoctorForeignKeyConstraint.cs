using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseClinic.Migrations
{
    /// <inheritdoc />
    public partial class FixDoctorForeignKeyConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Departments_DepartmentId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doctors_RequestedDoctorId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_PatientDelegations_TargetPatientId_ObserverAccountId",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Roles",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Queues",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Queues",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Prescriptions",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Prescriptions",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Permissions",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Patients",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "FullAddress",
                table: "Patients",
                newName: "AvatarUrl");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Patients",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "PatientDelegations",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PatientDelegations",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Medicines",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Medicines",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Encounters",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Encounters",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "DoctorSchedules",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "DoctorSchedules",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Doctors",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Doctors",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Departments",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Departments",
                newName: "FullDescription");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Departments",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Consultations",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Consultations",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Appointments",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Appointments",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Accounts",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiryTime",
                table: "Accounts",
                newName: "DeactivatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Accounts",
                newName: "UpdatedDate");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Roles",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "GrantedAt",
                table: "RolePermissions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "GrantedBy",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Queues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Queues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Queues",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Queues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Queues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Queues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Queues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Prescriptions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Prescriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Prescriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Prescriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Prescriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Resource",
                table: "Permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Permissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Permissions",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Permissions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Permissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PatientCode",
                table: "Patients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Patients",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duong",
                table: "Patients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HealthInsuranceNumber",
                table: "Patients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MergedIntoPatientId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "SoNha",
                table: "Patients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tinh",
                table: "Patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Xa",
                table: "Patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "PatientDelegations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "PatientDelegations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessLevel",
                table: "PatientDelegations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedByAccountId",
                table: "PatientDelegations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "PatientDelegations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "PatientDelegations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "GrantedAt",
                table: "PatientDelegations",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestedByAccountId",
                table: "PatientDelegations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevokeReason",
                table: "PatientDelegations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "PatientDelegations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Medicines",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Medicines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Medicines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Medicines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Medicines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Encounters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Encounters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Encounters",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Encounters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Encounters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Encounters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Encounters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "DoctorSchedules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "DoctorSchedules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "DoctorSchedules",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "DoctorSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "DoctorSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DoctorSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "DoctorSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "Doctors",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Doctors",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Doctors",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForOnline",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinDate",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "Doctors",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiryDate",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseIssueDate",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseIssuedBy",
                table: "Doctors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPatientsPerDay",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 999);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Doctors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Doctors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ClosingTime",
                table: "Departments",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Departments",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is24Hours",
                table: "Departments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Departments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Departments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpeningTime",
                table: "Departments",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Departments",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Departments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 999);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Consultations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Consultations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Consultations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Consultations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Consultations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "AppointmentCode",
                table: "Appointments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Appointments",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ContactRelationship",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Appointments",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Accounts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Accounts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeactivatedBy",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivationReason",
                table: "Accounts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePasswordOnNextLogin",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Accounts",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "vi");

            migrationBuilder.AddColumn<Guid>(
                name: "SecurityStamp",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AssignedAt",
                table: "AccountRoles",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedBy",
                table: "AccountRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AccountRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "LoginHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IpAddress = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginHistories_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtpCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 5)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpCodes_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientMergeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurvivorPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MergedPatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MergedSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MergedByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MergedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsRolledBack = table.Column<bool>(type: "bit", nullable: false),
                    RolledBackAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RolledBackByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RollbackReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMergeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMergeLogs_Patients_SurvivorPatientId",
                        column: x => x.SurvivorPatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsTrusted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MergeRollbackRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientMergeLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConflictResolutions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MergeRollbackRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MergeRollbackRequests_Accounts_RequestedByAccountId",
                        column: x => x.RequestedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MergeRollbackRequests_Accounts_ReviewedByAccountId",
                        column: x => x.ReviewedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MergeRollbackRequests_PatientMergeLogs_PatientMergeLogId",
                        column: x => x.PatientMergeLogId,
                        principalTable: "PatientMergeLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Code",
                table: "Roles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource",
                table: "Permissions",
                column: "Resource");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_FullName",
                table: "Patients",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_HealthInsuranceNumber",
                table: "Patients",
                column: "HealthInsuranceNumber",
                unique: true,
                filter: "[HealthInsuranceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MergedIntoPatientId",
                table: "Patients",
                column: "MergedIntoPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PrimaryPatientId",
                table: "Patients",
                column: "PrimaryPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDelegations_Status",
                table: "PatientDelegations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDelegations_TargetPatientId",
                table: "PatientDelegations",
                column: "TargetPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDelegations_TargetPatientId_ObserverAccountId",
                table: "PatientDelegations",
                columns: new[] { "TargetPatientId", "ObserverAccountId" },
                unique: true,
                filter: "[Status] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId_WorkDate_StartTime",
                table: "DoctorSchedules",
                columns: new[] { "DoctorId", "WorkDate", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_IsActive",
                table: "Doctors",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SortOrder",
                table: "Doctors",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsActive",
                table: "Departments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_SortOrder",
                table: "Departments",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentCode",
                table: "Appointments",
                column: "AppointmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentDate_DoctorId",
                table: "Appointments",
                columns: new[] { "AppointmentDate", "DoctorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Status",
                table: "Appointments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Status",
                table: "Accounts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistories_AccountId_LoginAt",
                table: "LoginHistories",
                columns: new[] { "AccountId", "LoginAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MergeRollbackRequests_PatientMergeLogId",
                table: "MergeRollbackRequests",
                column: "PatientMergeLogId");

            migrationBuilder.CreateIndex(
                name: "IX_MergeRollbackRequests_RequestedByAccountId",
                table: "MergeRollbackRequests",
                column: "RequestedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MergeRollbackRequests_ReviewedByAccountId",
                table: "MergeRollbackRequests",
                column: "ReviewedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MergeRollbackRequests_Status",
                table: "MergeRollbackRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_AccountId_Purpose_ExpiresAt",
                table: "OtpCodes",
                columns: new[] { "AccountId", "Purpose", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientMergeLogs_MergedAt",
                table: "PatientMergeLogs",
                column: "MergedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMergeLogs_MergedPatientId",
                table: "PatientMergeLogs",
                column: "MergedPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMergeLogs_SurvivorPatientId",
                table: "PatientMergeLogs",
                column: "SurvivorPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AccountId",
                table: "RefreshTokens",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Departments_DepartmentId",
                table: "Appointments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_RequestedDoctorId",
                table: "Appointments",
                column: "RequestedDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDelegations_Accounts_ObserverAccountId",
                table: "PatientDelegations",
                column: "ObserverAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDelegations_Patients_TargetPatientId",
                table: "PatientDelegations",
                column: "TargetPatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Patients_MergedIntoPatientId",
                table: "Patients",
                column: "MergedIntoPatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Patients_PrimaryPatientId",
                table: "Patients",
                column: "PrimaryPatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Departments_DepartmentId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doctors_RequestedDoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDelegations_Accounts_ObserverAccountId",
                table: "PatientDelegations");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDelegations_Patients_TargetPatientId",
                table: "PatientDelegations");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Patients_MergedIntoPatientId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Patients_PrimaryPatientId",
                table: "Patients");

            migrationBuilder.DropTable(
                name: "LoginHistories");

            migrationBuilder.DropTable(
                name: "MergeRollbackRequests");

            migrationBuilder.DropTable(
                name: "OtpCodes");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "PatientMergeLogs");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Code",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Code",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Resource",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Patients_FullName",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_HealthInsuranceNumber",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MergedIntoPatientId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PrimaryPatientId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_PatientDelegations_Status",
                table: "PatientDelegations");

            migrationBuilder.DropIndex(
                name: "IX_PatientDelegations_TargetPatientId",
                table: "PatientDelegations");

            migrationBuilder.DropIndex(
                name: "IX_PatientDelegations_TargetPatientId_ObserverAccountId",
                table: "PatientDelegations");

            migrationBuilder.DropIndex(
                name: "IX_DoctorSchedules_DoctorId_WorkDate_StartTime",
                table: "DoctorSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_IsActive",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_SortOrder",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Departments_IsActive",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_SortOrder",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentCode",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentDate_DoctorId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_Status",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Status",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "GrantedAt",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "GrantedBy",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Duong",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "HealthInsuranceNumber",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MergedIntoPatientId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PrimaryPatientId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RelationshipType",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "SoNha",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Tinh",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Xa",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AccessLevel",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "ApprovedByAccountId",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "GrantedAt",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "RequestedByAccountId",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "RevokeReason",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "PatientDelegations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsAvailableForOnline",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "JoinDate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "LicenseExpiryDate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "LicenseIssueDate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "LicenseIssuedBy",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "MaxPatientsPerDay",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Is24Hours",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "OpeningTime",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "AppointmentCode",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ContactRelationship",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CreatedId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DeactivatedBy",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DeactivationReason",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DeletedId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "MustChangePasswordOnNextLogin",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "UpdatedId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "AccountRoles");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "AccountRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AccountRoles");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Roles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Queues",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Queues",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Prescriptions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Prescriptions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Permissions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Patients",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Patients",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Patients",
                newName: "FullAddress");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "PatientDelegations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "PatientDelegations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Medicines",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Medicines",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Encounters",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Encounters",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "DoctorSchedules",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "DoctorSchedules",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Doctors",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Doctors",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Departments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "FullDescription",
                table: "Departments",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Departments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Consultations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Consultations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Appointments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Appointments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Accounts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Accounts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeactivatedAt",
                table: "Accounts",
                newName: "RefreshTokenExpiryTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Roles",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Queues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Queues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Prescriptions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Prescriptions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Resource",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Permissions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PatientCode",
                table: "Patients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                table: "Patients",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Patients",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "PatientDelegations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "PatientDelegations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Medicines",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Medicines",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Encounters",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Encounters",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "DoctorSchedules",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "DoctorSchedules",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "Doctors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Consultations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Consultations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                table: "Appointments",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenHash",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientDelegations_TargetPatientId_ObserverAccountId",
                table: "PatientDelegations",
                columns: new[] { "TargetPatientId", "ObserverAccountId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Departments_DepartmentId",
                table: "Appointments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_RequestedDoctorId",
                table: "Appointments",
                column: "RequestedDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

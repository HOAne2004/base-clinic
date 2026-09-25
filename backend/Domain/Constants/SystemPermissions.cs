using BaseClinic.Domain.Entities;

namespace BaseClinic.Domain.Constants
{
    public static class SystemPermissions
    {
        public static class Appointment
        {
            [PermissionInfo(Description = "Cho phép người dùng tạo lịch hẹn khám mới.")]
            public const string Book = "Appointment.Book";

            [PermissionInfo(Description = "Cho phép lễ tân check-in khi bệnh nhân đến.")]
            public const string CheckIn = "Appointment.CheckIn";

            [PermissionInfo(Description = "Cho phép người dùng, lễ tân hoặc bác sĩ hủy lịch.")]
            public const string Cancel = "Appointment.Cancel";
        }

        public static class Department
        {
            [PermissionInfo(Description = "Cho phép bệnh nhân / admin / lễ tân dùng xem danh sách khoa.")]
            public const string View = "Department.View";

            [PermissionInfo(Description = "Cho phép admin tạo khoa mới.")]
            public const string Create = "Department.Create";

            [PermissionInfo(Description = "Cho phép admin cập nhật khoa.")]
            public const string Update = "Department.Update";

            [PermissionInfo(Description = "Cho phép admin xóa mềm khoa.")]
            public const string Delete = "Department.Delete";
        }

        public static class Doctor
        {
            [PermissionInfo(Description = "Cho phép admin tạo tài khoản cho bác sĩ.")]
            public const string Create = "Doctor.Create";
        }
        public static class Encounter
        {
            [PermissionInfo(Description = "Bắt đầu phiên khám bệnh.")]
            public const string Start = "Encounter.Start";

            [PermissionInfo(Description = "Phiên khám hòan thành.")]
            public const string Complete = "Encounter.Complete";
        }
        public static class Permission
        {
            [PermissionInfo(Description = "Xem danh sách và chi tiết các quyền hiện có.")]
            public const string View = "Permission.View";
        }
        public static class Role
        {
            [PermissionInfo(Description = "Xem danh sách và chi tiết các chức danh hiện có.")]
            public const string View = "Role.View";

            [PermissionInfo(Description = "Tạo mới chức danh.")]
            public const string Create = "Role.Create";

            [PermissionInfo(Description = "Cập nhật thông tin và thay đổi phân quyền của chức danh.")]
            public const string Update = "Role.Update";

            [PermissionInfo(Description = "Xóa chức danh (chỉ áp dụng với các role không phải của hệ thống).")]
            public const string Delete = "Role.Delete";

        }

        public static class RolePermission
        {
            [PermissionInfo(Description = "Xem danh sách và chi tiết các quyền theo chức danh.")]
            public const string View = "RolePermission.View";

            [PermissionInfo(Description = "Gán quyền cho chức danh.")]
            public const string Assign = "RolePermission.Assign";
        }


    }
}

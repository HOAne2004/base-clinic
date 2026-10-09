namespace BaseClinic.Domain.Constants
{
    /// <summary>
    /// Định nghĩa toàn bộ permission của hệ thống.
    /// Format Code: {resource}:{action} — VD: "patient:view"
    /// </summary>
    public static class SystemPermissions
    {
        public static class Appointment
        {
            [PermissionInfo("Tạo lịch hẹn khám bệnh mới.", "Appointment", "Book", 1)]
            public const string Book = "appointment:book";

            [PermissionInfo("Xác nhận check-in khi bệnh nhân đến.", "Appointment", "CheckIn", 2)]
            public const string CheckIn = "appointment:checkin";

            [PermissionInfo("Hủy lịch hẹn khám bệnh.", "Appointment", "Cancel", 3)]
            public const string Cancel = "appointment:cancel";

            [PermissionInfo("Xem danh sách và chi tiết lịch hẹn.", "Appointment", "View", 4)]
            public const string View = "appointment:view";
        }

        public static class Department
        {
            [PermissionInfo("Xem thông tin khoa khám.", "Department", "View", 1)]
            public const string View = "department:view";

            [PermissionInfo("Tạo khoa khám bệnh mới.", "Department", "Create", 2)]
            public const string Create = "department:create";

            [PermissionInfo("Cập nhật thông tin khoa.", "Department", "Update", 3)]
            public const string Update = "department:update";

            [PermissionInfo("Xóa/vô hiệu hóa khoa.", "Department", "Delete", 4)]
            public const string Delete = "department:delete";
        }

        public static class Doctor
        {
            [PermissionInfo("Quản lý toàn diện hồ sơ bác sĩ.", "Doctor", "Manage", 1)]
            public const string Manage = "doctor:manage";

            [PermissionInfo("Xem thông tin bác sĩ.", "Doctor", "View", 2)]
            public const string View = "doctor:view";

            [PermissionInfo("Tạo hồ sơ bác sĩ mới.", "Doctor", "Create", 3)]
            public const string Create = "doctor:create";

            [PermissionInfo("Cập nhật hồ sơ bác sĩ.", "Doctor", "Update", 4)]
            public const string Update = "doctor:update";

            [PermissionInfo("Xóa hồ sơ bác sĩ.", "Doctor", "Delete", 5)]
            public const string Delete = "doctor:delete";
        }

        public static class Encounter
        {
            [PermissionInfo("Bắt đầu phiên khám mới.", "Encounter", "Start", 1)]
            public const string Start = "encounter:start";

            [PermissionInfo("Hoàn thành phiên khám.", "Encounter", "Complete", 2)]
            public const string Complete = "encounter:complete";
        }

        public static class Patient
        {
            [PermissionInfo("Xem hồ sơ bệnh nhân.", "Patient", "View", 1)]
            public const string View = "patient:view";

            [PermissionInfo("Tạo hồ sơ bệnh nhân mới.", "Patient", "Create", 2)]
            public const string Create = "patient:create";

            [PermissionInfo("Cập nhật hồ sơ bệnh nhân.", "Patient", "Update", 3)]
            public const string Update = "patient:update";

            [PermissionInfo("Quản lý ủy quyền theo dõi hồ sơ.", "Patient", "ManageDelegation", 4)]
            public const string ManageDelegation = "patient:manage_delegation";
        }

        public static class Permission
        {
            [PermissionInfo("Xem danh sách quyền trong hệ thống.", "Permission", "View", 1)]
            public const string View = "permission:view";
        }

        public static class Role
        {
            [PermissionInfo("Xem danh sách chức danh.", "Role", "View", 1)]
            public const string View = "role:view";

            [PermissionInfo("Tạo chức danh mới.", "Role", "Create", 2)]
            public const string Create = "role:create";

            [PermissionInfo("Cập nhật chức danh.", "Role", "Update", 3)]
            public const string Update = "role:update";

            [PermissionInfo("Xóa chức danh.", "Role", "Delete", 4)]
            public const string Delete = "role:delete";
        }

        public static class RolePermission
        {
            [PermissionInfo("Xem cấu hình quyền của chức danh.", "RolePermission", "View", 1)]
            public const string View = "role_permission:view";

            [PermissionInfo("Gán/thu hồi quyền cho chức danh.", "RolePermission", "Assign", 2)]
            public const string Assign = "role_permission:assign";
        }
    }
}
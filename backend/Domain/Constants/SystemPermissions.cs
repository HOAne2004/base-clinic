using BaseClinic.Domain.Entities;

namespace BaseClinic.Domain.Constants
{
    public static class SystemPermissions
    {
        public static class Appointment
        {
            [PermissionInfo(Description = "Tạo lịch hẹn khám bệnh mới vào hệ thống.")]
            public const string Book = "Appointment.Book";

            [PermissionInfo(Description = "Xác nhận check-in khi bệnh nhân đến phòng khám.")]
            public const string CheckIn = "Appointment.CheckIn";

            [PermissionInfo(Description = "Hủy lịch hẹn khám bệnh theo các điều kiện nghiệp vụ được phép.")]
            public const string Cancel = "Appointment.Cancel";

            [PermissionInfo(Description = "Xem danh sách và chi tiết lịch hẹn khám bệnh.")]
            public const string View = "Appointment.View";
        }

        public static class Department
        {
            [PermissionInfo(Description = "Xem danh sách và thông tin chi tiết khoa khám.")]
            public const string View = "Department.View";

            [PermissionInfo(Description = "Tạo khoa khám bệnh mới.")]
            public const string Create = "Department.Create";

            [PermissionInfo(Description = "Cập nhật thông tin khoa khám bệnh.")]
            public const string Update = "Department.Update";

            [PermissionInfo(Description = "Xóa hoặc vô hiệu hóa khoa khám bệnh.")]
            public const string Delete = "Department.Delete";
        }

        public static class Doctor
        {
            [PermissionInfo(Description = "Quản lý toàn diện hồ sơ và thông tin bảo mật của bác sĩ.")]
            public const string Manage = "Doctor.Manage";

            [PermissionInfo(Description = "Xem danh sách và thông tin cơ bản của bác sĩ.")]
            public const string View = "Doctor.View";

            [PermissionInfo(Description = "Tạo hồ sơ và tài khoản bác sĩ mới.")]
            public const string Create = "Doctor.Create";

            [PermissionInfo(Description = "Cập nhật thông tin hồ sơ bác sĩ.")]
            public const string Update = "Doctor.Update";

            [PermissionInfo(Description = "Xóa hồ sơ và vô hiệu hóa tài khoản bác sĩ.")]
            public const string Delete = "Doctor.Delete";
        }

        public static class Encounter
        {
            [PermissionInfo(Description = "Bắt đầu phiên khám bệnh mới.")]
            public const string Start = "Encounter.Start";

            [PermissionInfo(Description = "Hoàn thành phiên khám bệnh và chốt hồ sơ.")] // Đã sửa lỗi chính tả từ bản gốc
            public const string Complete = "Encounter.Complete";
        }

        public static class Patient
        {
            [PermissionInfo(Description = "Xem danh sách và chi tiết hồ sơ bệnh nhân trong phạm vi được phép.")]
            public const string View = "Patient.View";

            [PermissionInfo(Description = "Tạo hồ sơ bệnh nhân mới vào hệ thống.")]
            public const string Create = "Patient.Create";

            [PermissionInfo(Description = "Cập nhật thông tin hồ sơ bệnh nhân.")]
            public const string Update = "Patient.Update";

            [PermissionInfo(Description = "Quản lý danh sách người phụ thuộc và ủy quyền liên kết hồ sơ.")]
            public const string ManageDelegation = "Patient.ManageDelegation";
        }

        public static class Permission
        {
            [PermissionInfo(Description = "Xem danh sách và chi tiết các quyền hiện có trong hệ thống.")]
            public const string View = "Permission.View";
        }

        public static class Role
        {
            [PermissionInfo(Description = "Xem danh sách và chi tiết các chức danh hiện có.")]
            public const string View = "Role.View";

            [PermissionInfo(Description = "Tạo mới chức danh vào hệ thống.")]
            public const string Create = "Role.Create";

            [PermissionInfo(Description = "Cập nhật thông tin và cấu hình của chức danh.")]
            public const string Update = "Role.Update";

            [PermissionInfo(Description = "Xóa chức danh (không áp dụng với các chức danh hệ thống mặc định).")]
            public const string Delete = "Role.Delete";
        }

        public static class RolePermission
        {
            [PermissionInfo(Description = "Xem danh sách cấu hình quyền của từng chức danh.")]
            public const string View = "RolePermission.View";

            [PermissionInfo(Description = "Gán hoặc thu hồi quyền truy cập cho chức danh.")]
            public const string Assign = "RolePermission.Assign";
        }
    }
}
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
            [PermissionInfo(Description = "Cho phép người dùng tạo phòng ban mới.")]
            public const string Create = "Department.Create";
        }
        public static class Encounter
        {
            [PermissionInfo(Description = "Bắt đầu phiên khám bệnh.")]
            public const string Start = "Encounter.Start";

            [PermissionInfo(Description = "Phiên khám hòan thành.")]
            public const string Complete = "Encounter.Complete";
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

    }
}

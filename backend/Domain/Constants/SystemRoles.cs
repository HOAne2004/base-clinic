namespace BaseClinic.Domain.Constants
{
    public static class SystemRoles
    {
        // SuperAdmin: Seeder sẽ tự động gán TẤT CẢ permission, không cần khai báo
        public const string SuperAdmin = "SuperAdmin";

        // Admin: Quản lý user, role, department, doctor — không thao tác nghiệp vụ y tế
        [RolePermissions(
            SystemPermissions.Role.View,
            SystemPermissions.Role.Create,
            SystemPermissions.Role.Update,
            SystemPermissions.Role.Delete,
            SystemPermissions.Permission.View,
            SystemPermissions.RolePermission.View,
            SystemPermissions.RolePermission.Assign,
            SystemPermissions.Department.View,
            SystemPermissions.Department.Create,
            SystemPermissions.Department.Update,
            SystemPermissions.Department.Delete,
            SystemPermissions.Doctor.View,
            SystemPermissions.Doctor.Create,
            SystemPermissions.Doctor.Update,
            SystemPermissions.Doctor.Delete,
            SystemPermissions.Patient.View,
            SystemPermissions.Patient.RequestRollback,
            SystemPermissions.Patient.ApproveRollback
        )]
        public const string Admin = "Admin";

        // Doctor: Xem lịch hẹn, khám bệnh
        [RolePermissions(
            SystemPermissions.Appointment.View,
            SystemPermissions.Appointment.CheckIn,
            SystemPermissions.Encounter.Start,
            SystemPermissions.Encounter.Complete,
            SystemPermissions.Patient.View,
            SystemPermissions.Department.View,
            SystemPermissions.Doctor.View
        )]
        public const string Doctor = "Doctor";

        // Receptionist: Đặt lịch, check-in, quản lý hồ sơ bệnh nhân
        [RolePermissions(
            SystemPermissions.Appointment.View,
            SystemPermissions.Appointment.Book,
            SystemPermissions.Appointment.CheckIn,
            SystemPermissions.Appointment.Cancel,
            SystemPermissions.Patient.View,
            SystemPermissions.Patient.Create,
            SystemPermissions.Patient.Update,
            SystemPermissions.Department.View,
            SystemPermissions.Doctor.View,
            SystemPermissions.Patient.RequestRollback
        )]
        public const string Receptionist = "Receptionist";

        // Patient: Đặt lịch, hủy lịch, xem hồ sơ của mình, quản lý người phụ thuộc
        [RolePermissions(
            SystemPermissions.Appointment.View,
            SystemPermissions.Appointment.Book,
            SystemPermissions.Appointment.Cancel,
            SystemPermissions.Patient.View,
            SystemPermissions.Patient.ManageDelegation,
            SystemPermissions.Department.View,
            SystemPermissions.Doctor.View,
            SystemPermissions.Patient.RequestRollback
        )]
        public const string Patient = "Patient";
    }
}
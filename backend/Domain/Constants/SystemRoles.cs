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
            SystemPermissions.Department.Deactivate,   
            SystemPermissions.Department.Activate,    
            SystemPermissions.Department.Restore,

            SystemPermissions.Doctor.Manage,
            SystemPermissions.Doctor.View,
            SystemPermissions.Doctor.Create,
            SystemPermissions.Doctor.Update,
            SystemPermissions.Doctor.Delete,
            SystemPermissions.Doctor.Activate,        
            SystemPermissions.Doctor.Deactivate,            
            SystemPermissions.Doctor.TransferDepartment,

            SystemPermissions.DoctorSchedule.View,
            SystemPermissions.DoctorSchedule.Create,
            SystemPermissions.DoctorSchedule.Update,
            SystemPermissions.DoctorSchedule.Delete,
            SystemPermissions.DoctorSchedule.AutoFill,
            SystemPermissions.DoctorSchedule.Complete,
            SystemPermissions.DoctorSchedule.Cancel,
            SystemPermissions.DoctorSchedule.RequestCancel,
            SystemPermissions.DoctorSchedule.ApproveCancel,
            SystemPermissions.DoctorSchedule.RejectCancel,
            SystemPermissions.DoctorSchedule.BlockSlot,

            SystemPermissions.Patient.View,
            SystemPermissions.Patient.RequestRollback,
            SystemPermissions.Patient.ApproveRollback
        )]
        public const string Admin = "Admin";

        // Doctor: Xem lịch hẹn, khám bệnh
        [RolePermissions(
            SystemPermissions.Appointment.View,
            SystemPermissions.Appointment.CheckIn,

            SystemPermissions.Department.View,

            SystemPermissions.Doctor.View,
            SystemPermissions.Doctor.UpdateSelf,

            SystemPermissions.DoctorSchedule.View,
            SystemPermissions.DoctorSchedule.Create,
            SystemPermissions.DoctorSchedule.Update,
            SystemPermissions.DoctorSchedule.AutoFill,
            SystemPermissions.DoctorSchedule.Complete,
            SystemPermissions.DoctorSchedule.Cancel,
            SystemPermissions.DoctorSchedule.RequestCancel,

            SystemPermissions.Encounter.Start,
            SystemPermissions.Encounter.Complete,

            SystemPermissions.Patient.View
        )]
        public const string Doctor = "Doctor";

        // Receptionist: Đặt lịch, check-in, quản lý hồ sơ bệnh nhân
        [RolePermissions(
            SystemPermissions.Appointment.View,
            SystemPermissions.Appointment.Book,
            SystemPermissions.Appointment.CheckIn,
            SystemPermissions.Appointment.Cancel,

            SystemPermissions.Department.View,

            SystemPermissions.Doctor.View,

            SystemPermissions.DoctorSchedule.View,
            SystemPermissions.DoctorSchedule.Create,
            SystemPermissions.DoctorSchedule.Update,
            SystemPermissions.DoctorSchedule.AutoFill,
            SystemPermissions.DoctorSchedule.Complete,
            SystemPermissions.DoctorSchedule.Cancel,
            SystemPermissions.DoctorSchedule.RequestCancel,
            SystemPermissions.DoctorSchedule.BlockSlot,

            SystemPermissions.Patient.View,
            SystemPermissions.Patient.Create,
            SystemPermissions.Patient.Update,
            SystemPermissions.Patient.RequestRollback

        )]
        public const string Receptionist = "Receptionist";

        // Patient: Đặt lịch, hủy lịch, xem hồ sơ của mình, quản lý người phụ thuộc
        [RolePermissions(
            SystemPermissions.Appointment.View,
            SystemPermissions.Appointment.Book,
            SystemPermissions.Appointment.Cancel,



            SystemPermissions.Department.View,

            SystemPermissions.Doctor.View,

            SystemPermissions.DoctorSchedule.View,

            SystemPermissions.Patient.View,
            SystemPermissions.Patient.ManageDelegation,
            SystemPermissions.Patient.RequestRollback
        )]
        public const string Patient = "Patient";
    }
}
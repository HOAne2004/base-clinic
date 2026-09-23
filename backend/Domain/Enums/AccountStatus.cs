namespace BaseClinic.Domain.Enums
{
    public enum  AccountStatus
    {
        Active = 1, //Tài khoản đang sử dụng bình thường
        Inactive = 2, //Tài khoản chưa kích hoạt / không hoạt động
        Suspended = 3 // Bị khóa/tạm ngưng bởi hệ thống
    }
}

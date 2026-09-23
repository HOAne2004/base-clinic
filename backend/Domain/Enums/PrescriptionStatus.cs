namespace BaseClinic.Domain.Enums
{
    public enum PrescriptionStatus
    {
        Pending = 1, //Đơn thuốc đang chờ xử lý
        Approved = 2, //Đơn thuốc đã được duyệt
        Active = 3, //Đơn thuốc đã được phát thuốc
        Cancelled = 4, //Đơn thuốc đã bị hủy
        Expired = 5 //Đơn thuốc đã hết hạn
    }
}

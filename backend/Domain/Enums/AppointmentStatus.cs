namespace BaseClinic.Domain.Enums
{
    public enum AppointmentStatus
    {
        Confirmed = 1, // Lịch hẹn đã được xác nhận bởi bệnh nhân và bác sĩ
        CheckedIn = 2, // Bệnh nhân đã đến và check-in tại phòng khám
        Completed = 3, // Lịch hẹn đã hoàn tất, bác sĩ đã khám xong cho bệnh nhân
        Cancelled = 4, // Lịch hẹn đã bị hủy bởi bệnh nhân hoặc bác sĩ
        NoShow = 5 // Bệnh nhân không đến đúng giờ và không thông báo hủy lịch hẹn
    }
}

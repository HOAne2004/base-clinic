namespace BaseClinic.Domain.Enums
{
    public enum DoctorScheduleStatus
    {
        Available = 1, //Lịch đang mở và còn khả năng nhận bệnh nhân
        Full = 2, //Lịch đã đầy, không còn khả năng nhận bệnh nhân
        Cancelled = 3, //Lịch đã bị hủy bởi bác sĩ hoặc hệ thống
        Completed = 4 //Lịch đã kết thúc, bác sĩ đã hoàn thành công việc trong lịch
    }
}

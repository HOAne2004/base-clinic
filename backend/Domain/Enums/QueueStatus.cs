namespace BaseClinic.Domain.Enums
{
    public enum QueueStatus
    {
        Open = 1, //Hàng đợi đang mở và còn khả năng nhận bệnh nhân
        Paused = 2, //Hàng đợi tạm dừng, không nhận bệnh nhân mới nhưng vẫn phục vụ những bệnh nhân đã có trong hàng đợi
        Closed = 3 //Hàng đợi đã đóng, không nhận bệnh nhân mới và không phục vụ những bệnh nhân đã có trong hàng đợi
    }
}

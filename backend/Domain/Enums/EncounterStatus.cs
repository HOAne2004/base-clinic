namespace BaseClinic.Domain.Enums
{
    public enum EncounterStatus
    {
        Waiting = 1, //Đã check-in nhưng chưa bắt đầu khám
        InProgress = 2, //Đang trong quá trình khám
        Completed = 3, //Đã hoàn tất quá trình khám 
        Cancelled = 4 //Quá trình khám đã bị hủy bởi bác sĩ hoặc bệnh nhân
    }
}

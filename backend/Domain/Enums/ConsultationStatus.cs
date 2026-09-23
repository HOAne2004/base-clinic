namespace BaseClinic.Domain.Enums
{
    public enum ConsultationStatus
    {
        InProgress = 1, // Cuộc tư vấn đang diễn ra
        Completed = 2, // Cuộc tư vấn đã hoàn tất
        Cancelled = 3 // Cuộc tư vấn đã bị hủy bởi bác sĩ hoặc bệnh nhân
    }
}

namespace BaseClinic.Business.Models
{
    public class CheckInResultDto
    {
        public Guid EncounterId { get; set; }
        public string EncounterCode { get; set; } = string.Empty;
        public Guid QueueId { get; set; }
        public int QueueNumber { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
    }
}

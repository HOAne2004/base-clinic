namespace BaseClinic.Business.Models
{
    public record CheckInResultDto
    (
        Guid EncounterId,
        string EncounterCode,
        Guid QueueId,
        int QueueNumber,
        string DepartmentName,
        string PatientName,
        DateTime CheckInTime
        );

}

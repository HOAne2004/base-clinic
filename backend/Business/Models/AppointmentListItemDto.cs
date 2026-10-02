namespace BaseClinic.Business.Models
{
    public record AppointmentListItemDto(
            Guid Id,
            Guid PatientId,
            string PatientName,
            string? DepartmentName,
            string? DoctorName,
            DateTime AppointmentDate,
            TimeOnly StartTime,
            TimeOnly EndTime,
            string Status
        );
}

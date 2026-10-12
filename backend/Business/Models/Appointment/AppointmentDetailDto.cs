using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.Appointment
{
    public record AppointmentDetailDto(
        Guid Id,
        Guid PatientId,
        string PatientName,
        Guid? DepartmentId,
        string? DepartmentName,
        Guid? RequestedDoctorId,
        string? DoctorName,
        DateTime AppointmentDate,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string Reason,
        AppointmentStatus Status,
        string? CancelReason
    );
}

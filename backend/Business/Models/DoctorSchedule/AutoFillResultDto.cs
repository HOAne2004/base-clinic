namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Kết quả sau khi auto-fill.
    /// </summary>
    public record AutoFillResultDto(
        int CreatedSchedules,
        int SkippedDays,
        List<DateOnly> CreatedDates,
        List<DateOnly> SkippedDates
    );
}
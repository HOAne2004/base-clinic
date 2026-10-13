namespace BaseClinic.Business.Models.Doctor
{
    /// <summary>
    /// Kết quả deactivate — có thể kèm cảnh báo.
    /// </summary>
    public record DeactivateDoctorResultDto(
        bool Succeeded,
        List<string> Warnings
    );
}
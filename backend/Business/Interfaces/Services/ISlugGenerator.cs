namespace BaseClinic.Business.Interfaces.Services
{
    /// <summary>
    /// Sinh slug unique — có check DB.
    /// Khác với SlugHelper (chỉ generate, không check DB).
    /// </summary>
    public interface ISlugGenerator
    {
        /// <summary>
        /// Sinh slug unique cho Department.
        /// Nếu trùng → append -2, -3...
        /// </summary>
        /// <param name="input">Tên nguồn (VD: "Khoa Nội").</param>
        /// <param name="excludeId">Loại trừ Id khi check (dùng khi update).</param>
        Task<string> GenerateUniqueDepartmentSlugAsync(
            string input,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sinh slug unique cho Doctor.
        /// </summary>
        Task<string> GenerateUniqueDoctorSlugAsync(
            string input,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);
    }
}
namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Kết quả tạo bệnh nhân.
    /// </summary>
    public record CreatePatientResultDto(
        Guid PatientId,
        string PatientCode,
        Guid? AccountId,

        /// <summary>
        /// Mật khẩu mặc định (nếu có tạo Account).
        /// Null nếu chỉ tạo Patient guest hoặc dependent.
        /// FE hiển thị cho lễ tân để đọc/gửi cho bệnh nhân.
        /// </summary>
        string? DefaultPassword,

        /// <summary>
        /// True nếu Account cần verify SĐT trước khi dùng.
        /// FE hiển thị thông báo: "Bệnh nhân cần xác thực SĐT khi đăng nhập lần đầu."
        /// </summary>
        bool RequiresPhoneVerification
    );
}
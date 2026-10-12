using System.ComponentModel.DataAnnotations;
using BaseClinic.Business.Models.Common;

namespace BaseClinic.Business.Models.Patient
{
    /// <summary>
    /// Request tạo bệnh nhân mới.
    /// 
    /// 3 TRƯỜNG HỢP:
    /// 
    /// ┌─────────────────────────────────────────────────────────────────┐
    /// │ 1. WALK-IN CÓ SĐT CỦA CHÍNH PATIENT                             │
    /// │    - PrimaryPatientId = null                                    │
    /// │    - PhoneNumber != null (là SĐT của patient)                   │
    /// │    → Tạo Patient + Account (password = SĐT) + gán Role PATIENT  │
    /// │    → Account Inactive (verify sau)                              │
    /// ├─────────────────────────────────────────────────────────────────┤
    /// │ 2. WALK-IN KHÔNG CÓ SĐT (hoặc SĐT là của người đưa đi)          │
    /// │    - PrimaryPatientId = null                                    │
    /// │    - PhoneNumber = null                                         │
    /// │    → Chỉ tạo Patient guest                                      │
    /// │    → Thông tin người đưa đi lưu ở Appointment (Contact*)        │
    /// │    → Ví dụ: bố/mẹ đưa con 5 tuổi đi khám                        │
    /// ├─────────────────────────────────────────────────────────────────┤
    /// │ 3. DEPENDENT (đã có patient chính trong hệ thống)               │
    /// │    - PrimaryPatientId != null                                   │
    /// │    - PhoneNumber phải = null (dependent không tạo Account)      │
    /// │    → Tạo Patient, liên kết với Primary                          │
    /// │    → Ví dụ: A có Account, tạo hồ sơ cho con B                   │
    /// └─────────────────────────────────────────────────────────────────┘
    /// </summary>
    public class CreatePatientDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// SĐT CỦA CHÍNH PATIENT (không phải SĐT của người đưa đi).
        /// 
        /// - Có SĐT → tạo Account cho Patient với password = SĐT.
        /// - Không có → Patient guest, không Account.
        /// 
        /// LƯU Ý: Nếu bố/mẹ đưa con đi khám và SĐT là của bố/mẹ,
        /// KHÔNG nhập vào đây. SĐT đó sẽ được nhập khi đặt Appointment
        /// (ContactPhone).
        /// </summary>
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? IdentityNumber { get; set; }

        [StringLength(20)]
        public string? HealthInsuranceNumber { get; set; }

        public AddressDto? Address { get; set; }

        /// <summary>
        /// Null → walk-in (primary).
        /// Có giá trị → dependent của Patient tương ứng.
        /// </summary>
        public Guid? PrimaryPatientId { get; set; }
    }
}
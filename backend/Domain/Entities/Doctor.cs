using BaseClinic.Domain.Common;
using System;
using System.Text.RegularExpressions;

namespace BaseClinic.Domain.Entities
{
    public class Doctor : FullAuditedEntity
    {
        private const int MinDoctorAge = 22;
        private const int MaxDoctorAge = 75;
        private const decimal MaxConsultationFee = 10_000_000m;

        public Guid AccountId { get; private set; }
        public Guid DepartmentId { get; private set; }
        public string DoctorCode { get; private set; } = null!;

        // === Thông tin hành nghề ===
        public string? Title { get; private set; }              // BS, ThS, TS, PGS, GS
        public string? Specialization { get; private set; }     // Chuyên khoa sâu
        public int? YearsOfExperience { get; private set; }
        public string? Biography { get; private set; }          // Tiểu sử dài
        public string? Languages { get; private set; }          // "vi,en"

        // === Thông tin cá nhân ===
        public string? Avatar { get; private set; }
        public string? FullAddress { get; private set; }
        public bool? Gender { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string? IdentityNumber { get; private set; }

        // === Pháp lý ===
        public string LicenseNumber { get; private set; } = null!;   // BẮT BUỘC
        public DateTime? LicenseIssueDate { get; private set; }
        public DateTime? LicenseExpiryDate { get; private set; }
        public string? LicenseIssuedBy { get; private set; }

        // === Tài chính & vận hành ===
        public decimal? ConsultationFee { get; private set; }
        public bool IsAvailableForOnline { get; private set; } = true;
        public int? MaxPatientsPerDay { get; private set; }
        public DateTime? JoinDate { get; private set; }

        public string? Description { get; private set; }

        private Doctor() { }

        public Doctor(Guid accountId, Guid departmentId, string doctorCode,
                      string licenseNumber, DateTime? licenseIssueDate, DateTime? licenseExpiryDate,
                      string? licenseIssuedBy)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("AccountId không hợp lệ.");
            if (departmentId == Guid.Empty) throw new ArgumentException("DepartmentId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(doctorCode)) throw new ArgumentException("DoctorCode không được để trống.");

            AccountId = accountId;
            DepartmentId = departmentId;
            DoctorCode = doctorCode.Trim().ToUpperInvariant();

            UpdateLicense(licenseNumber, licenseIssueDate, licenseExpiryDate, licenseIssuedBy);
        }

        // ==== PROFILE ====
        public void UpdateProfile(string? avatar, string? fullAddress, bool? gender,
                                  DateTime? dateOfBirth, string? identityNumber, string? description)
        {
            ValidateDateOfBirth(dateOfBirth);

            if (!string.IsNullOrWhiteSpace(identityNumber) &&
                !Regex.IsMatch(identityNumber, @"^\d{9}$|^\d{12}$"))
                throw new ArgumentException("Số CMND/CCCD phải là 9 hoặc 12 chữ số.", nameof(identityNumber));

            Avatar = avatar?.Trim();
            FullAddress = fullAddress?.Trim();
            Gender = gender;
            DateOfBirth = dateOfBirth;
            IdentityNumber = identityNumber?.Trim();
            Description = description?.Trim();
        }

        // ==== NGHỀ NGHIỆP ====
        public void UpdateProfessionalInfo(string? title, string? specialization,
                                           int? yearsOfExperience, string? biography, string? languages)
        {
            if (yearsOfExperience.HasValue && (yearsOfExperience < 0 || yearsOfExperience > 60))
                throw new ArgumentException("Số năm kinh nghiệm không hợp lệ.", nameof(yearsOfExperience));

            Title = title?.Trim();
            Specialization = specialization?.Trim();
            YearsOfExperience = yearsOfExperience;
            Biography = biography?.Trim();
            Languages = languages?.Trim();
        }

        public void TransferToDepartment(Guid newDepartmentId)
        {
            if (newDepartmentId == Guid.Empty)
                throw new ArgumentException("DepartmentId mới không hợp lệ.");
            DepartmentId = newDepartmentId;
        }

        public void UpdateConsultationFee(decimal fee)
        {
            if (fee < 0) throw new ArgumentException("Phí khám không được âm.");
            if (fee > MaxConsultationFee) throw new ArgumentException($"Phí khám không được vượt quá {MaxConsultationFee:N0} VNĐ.");
            ConsultationFee = fee;
        }

        // ==== PHÁP LÝ ====
        public void UpdateLicense(string licenseNumber, DateTime? issueDate,
                                  DateTime? expiryDate, string? issuedBy)
        {
            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("Số chứng chỉ hành nghề không được để trống.");

            if (issueDate.HasValue && expiryDate.HasValue && issueDate >= expiryDate)
                throw new ArgumentException("Ngày cấp phải trước ngày hết hạn.");

            if (expiryDate.HasValue && expiryDate.Value.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Chứng chỉ hành nghề đã hết hạn.");

            LicenseNumber = licenseNumber.Trim();
            LicenseIssueDate = issueDate;
            LicenseExpiryDate = expiryDate;
            LicenseIssuedBy = issuedBy?.Trim();
        }

        public bool IsLicenseExpiringSoon(int days = 30)
            => LicenseExpiryDate.HasValue
               && LicenseExpiryDate.Value.Date <= DateTime.UtcNow.Date.AddDays(days);

        // ==== VẬN HÀNH ====
        public void SetOnlineAvailability(bool isAvailable) => IsAvailableForOnline = isAvailable;

        public void UpdateWorkload(int? maxPatientsPerDay, DateTime? joinDate)
        {
            if (maxPatientsPerDay.HasValue && (maxPatientsPerDay < 0 || maxPatientsPerDay > 200))
                throw new ArgumentException("Số bệnh nhân tối đa/ngày không hợp lệ.");
            MaxPatientsPerDay = maxPatientsPerDay;
            JoinDate = joinDate;
        }

        // ==== HELPERS ====
        private static void ValidateDateOfBirth(DateTime? dob)
        {
            if (!dob.HasValue) return;

            var today = DateTime.UtcNow.Date;
            if (dob.Value.Date > today)
                throw new ArgumentException("Ngày sinh không thể ở tương lai.");

            var age = today.Year - dob.Value.Year;
            if (dob.Value.Date > today.AddYears(-age)) age--;

            if (age < MinDoctorAge)
                throw new ArgumentException($"Bác sĩ phải từ {MinDoctorAge} tuổi trở lên.");
            if (age > MaxDoctorAge)
                throw new ArgumentException($"Ngày sinh không hợp lệ (vượt quá {MaxDoctorAge} tuổi).");
        }
    }
}
using BaseClinic.Domain.Common;
using System;

namespace BaseClinic.Domain.Entities
{
    public class Doctor : AggregateRoot
    {
        public Guid AccountId { get; private set; }
        public Guid DepartmentId { get; private set; } // Bổ sung DepartmentId
        public string DoctorCode { get; private set; } = null!;
        public string? Avatar { get; private set; }
        public string? FullAddress { get; private set; }
        public bool? Gender { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string? IdentityNumber { get; private set; }
        public string? LicenseNumber { get; private set; }
        public decimal? ConsultationFee { get; private set; }
        public string? Description { get; private set; }

        private Doctor() { } // Dành cho ORM

        public Doctor(Guid accountId, Guid departmentId, string doctorCode, string? avatar, string? fullAddress, bool? gender, DateTime? dateOfBirth, string? identityNumber, string? licenseNumber, decimal? consultationFee, string? description)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("AccountId không hợp lệ.", nameof(accountId));
            if (departmentId == Guid.Empty) throw new ArgumentException("DepartmentId không hợp lệ.", nameof(departmentId));
            if (string.IsNullOrWhiteSpace(doctorCode)) throw new ArgumentException("DoctorCode không được để trống.", nameof(doctorCode));

            AccountId = accountId;
            DepartmentId = departmentId;
            DoctorCode = doctorCode.Trim(); // Thường Immutable sau khi cấp

            UpdateProfile(avatar, fullAddress, gender, dateOfBirth, identityNumber, description);
            UpdateConsultationFee(consultationFee ?? 0);
            UpdateLicenseNumber(licenseNumber);
        }

        // Tách riêng cập nhật profile cơ bản
        public void UpdateProfile(string? avatar, string? fullAddress, bool? gender, DateTime? dateOfBirth, string? identityNumber, string? description)
        {
            if (dateOfBirth.HasValue && dateOfBirth.Value.Date > DateTime.UtcNow.Date)
                throw new ArgumentException("Ngày sinh không thể ở thì tương lai.", nameof(dateOfBirth));

            Avatar = avatar?.Trim();
            FullAddress = fullAddress?.Trim();
            Gender = gender;
            DateOfBirth = dateOfBirth;
            IdentityNumber = identityNumber?.Trim();
            Description = description?.Trim();
        }

        // Tách riêng nghiệp vụ đổi khoa
        public void TransferToDepartment(Guid newDepartmentId)
        {
            if (newDepartmentId == Guid.Empty)
                throw new ArgumentException("DepartmentId mới không hợp lệ.", nameof(newDepartmentId));

            DepartmentId = newDepartmentId;
        }

        // Tách riêng cập nhật phí khám kèm validation
        public void UpdateConsultationFee(decimal fee)
        {
            if (fee < 0)
                throw new ArgumentException("Phí khám bệnh không được là số âm.", nameof(fee));

            ConsultationFee = fee;
        }

        // Tách riêng cập nhật chứng chỉ hành nghề (có thể mở rộng thêm logic duyệt/log sau này)
        public void UpdateLicenseNumber(string? licenseNumber)
        {
            LicenseNumber = licenseNumber?.Trim();
        }
    }
}
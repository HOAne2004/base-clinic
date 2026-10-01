using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using System;

namespace BaseClinic.Domain.Entities
{
    public class Patient : AggregateRoot
    {
        public Guid? AccountId { get; private set; }
        public string PatientCode { get; private set; } = null!;
        public string? FullName { get; private set; }
        public string? Avatar { get; private set; }
        public string? FullAddress { get; private set; }
        public string? IdentityNumber { get; private set; }
        public bool? Gender { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public bool? IsPrimary { get; private set; }

        private Patient() { }

        public Patient(string patientCode, Guid? accountId, string fullName, bool isPrimary)
        {
            if (string.IsNullOrWhiteSpace(patientCode))
                throw new ArgumentException("Mã bệnh nhân không được để trống.");

            PatientCode = patientCode;
            AccountId = accountId;
            SetFullName(fullName);
            IsPrimary = isPrimary;
        }

        public void UpdateProfile(string fullName, string? avatar, string? fullAddress, string? identityNumber, bool? gender, DateTime? dateOfBirth)
        {
            SetFullName(fullName);
            Avatar = avatar;
            FullAddress = fullAddress;
            IdentityNumber = identityNumber;
            Gender = gender;

            if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.UtcNow)
                throw new ArgumentException("Ngày sinh không thể nằm ở tương lai.");
            DateOfBirth = dateOfBirth;
        }

        // Bổ sung hàm để gán AccountId khi bệnh nhân Claim Profile (UC-04)
        public void ClaimProfile(Guid accountId)
        {
            if (AccountId.HasValue)
                throw new InvalidOperationException("Hồ sơ này đã được liên kết với một tài khoản khác.");

            AccountId = accountId;
        }

        private void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Tên bệnh nhân không được để trống.");
            if (fullName.Length > 100)
                throw new ArgumentException("Tên bệnh nhân không được vượt quá 100 ký tự.");
            FullName = fullName.Trim();
        }
    }
}
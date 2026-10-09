using BaseClinic.Domain.Common;
using BaseClinic.Domain.Exceptions;
using BaseClinic.Domain.ValueObjects;

namespace BaseClinic.Domain.Entities
{
    /// <summary>
    /// Hồ sơ bệnh nhân.
    /// 
    /// QUAN HỆ:
    /// - Account (0..1): Account sở hữu hồ sơ (nullable — chưa claim).
    /// - PrimaryPatient (0..1): Patient chính mà Patient này phụ thuộc.
    /// - Dependents (0..N): Danh sách Patient phụ thuộc Patient này.
    /// - Delegations (0..N): Danh sách ủy quyền theo dõi.
    /// 
    /// LƯU Ý: KHÔNG chứa thông tin liên hệ (Phone, ContactPhone, Email).
    /// Các thông tin đó nằm ở Account (identity) hoặc Appointment (snapshot).
    /// </summary>
    public class Patient : FullAuditedEntity
    {
        // ==================== LIÊN KẾT ACCOUNT ====================
        public Guid? AccountId { get; private set; }
        public Account? Account { get; private set; }

        // ==================== QUAN HỆ PATIENT → PATIENT ====================
        /// <summary>
        /// Patient chính mà Patient này phụ thuộc (nullable).
        /// - Null: Patient là primary (chính chủ).
        /// - Có: Patient là dependent của một Patient khác.
        /// </summary>
        public Guid? PrimaryPatientId { get; private set; }
        public Patient? PrimaryPatient { get; private set; }

        private readonly List<Patient> _dependents = new();
        public IReadOnlyCollection<Patient> Dependents => _dependents.AsReadOnly();

        // ==================== THÔNG TIN HỒ SƠ ====================
        /// <summary>Mã bệnh nhân — unique, sinh tự động. VD: "BN2026100001".</summary>
        public string PatientCode { get; private set; } = string.Empty;

        /// <summary>Họ tên bệnh nhân.</summary>
        public string FullName { get; private set; } = string.Empty;

        /// <summary>URL ảnh đại diện.</summary>
        public string? AvatarUrl { get; private set; }

        /// <summary>Địa chỉ — Value Object (Owned Entity).</summary>
        public Address Address { get; private set; } = Address.Empty;

        /// <summary>Số CCCD/CMND.</summary>
        public string? IdentityNumber { get; private set; }

        /// <summary>Giới tính.</summary>
        public bool? Gender { get; private set; }

        /// <summary>Ngày sinh.</summary>
        public DateTime? DateOfBirth { get; private set; }

        /// <summary>Số BHYT.</summary>
        public string? HealthInsuranceNumber { get; private set; }

        /// <summary>
        /// Đánh dấu Patient này là "chính chủ" của Account.
        /// - True: Patient A khi A đăng ký tài khoản.
        /// - False: Patient dependent (con, người được quản lý hộ).
        /// </summary>
        public bool IsPrimary { get; private set; }

        // ==================== NAVIGATION ====================
        private readonly List<PatientDelegation> _delegations = new();
        public IReadOnlyCollection<PatientDelegation> Delegations => _delegations.AsReadOnly();

        private Patient() { } // EF Core

        /// <summary>
        /// Tạo Patient mới.
        /// </summary>
        public Patient(
            string patientCode,
            string fullName,
            Guid? accountId = null,
            Guid? primaryPatientId = null,
            bool isPrimary = false)
        {
            SetPatientCode(patientCode);
            SetFullName(fullName);

            if (isPrimary && primaryPatientId != null)
                throw new DomainException("Patient chính không thể có PrimaryPatientId.");

            if (!isPrimary && primaryPatientId == null && accountId == null)
                throw new DomainException(
                    "Patient phải là primary HOẶC có PrimaryPatientId HOẶC có AccountId.");

            AccountId = accountId;
            PrimaryPatientId = primaryPatientId;
            IsPrimary = isPrimary;
            Address = Address.Empty;
        }

        // ==================== PROFILE METHODS ====================
        private void SetPatientCode(string patientCode)
        {
            if (string.IsNullOrWhiteSpace(patientCode))
                throw new DomainException("Mã bệnh nhân không được để trống.");
            if (patientCode.Length > 50)
                throw new DomainException("Mã bệnh nhân không vượt quá 50 ký tự.");

            PatientCode = patientCode.Trim();
        }

        private void SetFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Tên bệnh nhân không được để trống.");
            if (fullName.Length > 100)
                throw new DomainException("Tên bệnh nhân không vượt quá 100 ký tự.");

            FullName = fullName.Trim();
        }

        /// <summary>Cập nhật thông tin hồ sơ (trừ mã BN, Account, quan hệ).</summary>
        public void UpdateProfile(
            string fullName,
            string? avatarUrl,
            Address? address,
            string? identityNumber,
            bool? gender,
            DateTime? dateOfBirth,
            string? healthInsuranceNumber = null)
        {
            SetFullName(fullName);

            if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.UtcNow)
                throw new DomainException("Ngày sinh không thể nằm ở tương lai.");

            if (identityNumber?.Length > 20)
                throw new DomainException("Số CCCD không vượt quá 20 ký tự.");

            if (healthInsuranceNumber?.Length > 20)
                throw new DomainException("Số BHYT không vượt quá 20 ký tự.");

            AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();
            Address = address ?? Address.Empty;
            IdentityNumber = string.IsNullOrWhiteSpace(identityNumber) ? null : identityNumber.Trim();
            Gender = gender;
            DateOfBirth = dateOfBirth;
            HealthInsuranceNumber = string.IsNullOrWhiteSpace(healthInsuranceNumber)
                ? null
                : healthInsuranceNumber.Trim();
        }

        // ==================== CLAIM ACCOUNT ====================
        /// <summary>Claim hồ sơ — gán Account cho Patient.</summary>
        public void ClaimByAccount(Guid accountId)
        {
            if (accountId == Guid.Empty)
                throw new DomainException("AccountId không hợp lệ.");
            if (AccountId.HasValue)
                throw new DomainException("Hồ sơ này đã được liên kết với một tài khoản khác.");

            AccountId = accountId;
        }

        /// <summary>Bỏ liên kết Account (chỉ áp dụng cho dependent).</summary>
        public void UnlinkAccount()
        {
            if (IsPrimary)
                throw new DomainException("Không thể bỏ liên kết Account của hồ sơ chính chủ.");

            AccountId = null;
        }

        // ==================== QUAN HỆ PATIENT → PATIENT ====================
        public void SetPrimaryPatient(Guid primaryPatientId)
        {
            if (primaryPatientId == Guid.Empty)
                throw new DomainException("PrimaryPatientId không hợp lệ.");
            if (primaryPatientId == Id)
                throw new DomainException("Patient không thể là primary của chính mình.");
            if (IsPrimary)
                throw new DomainException("Không thể gán primary cho Patient chính chủ.");

            PrimaryPatientId = primaryPatientId;
        }

        public void RemovePrimaryPatient()
        {
            if (IsPrimary)
                throw new DomainException("Patient chính không có PrimaryPatientId để xóa.");

            PrimaryPatientId = null;
        }

        // ==================== DELEGATION ====================
        internal void AddDelegation(PatientDelegation delegation)
        {
            if (delegation == null)
                throw new DomainException("Delegation không được null.");

            _delegations.Add(delegation);
        }

        internal void RemoveDelegation(PatientDelegation delegation)
            => _delegations.Remove(delegation);
    }
}
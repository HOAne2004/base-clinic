using BaseClinic.Business.Models.Doctor;
using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Mappers
{
    /// <summary>
    /// Helper chuyển Doctor entity sang các DTO.
    /// 
    /// QUY ƯỚC:
    /// - Doctor KHÔNG có FullName → phải truyền accountName.
    /// - Age tính từ DateOfBirth.
    /// - Department info phải load riêng và truyền vào.
    /// </summary>
    public static class DoctorMapper
    {
        // ==================== PUBLIC ====================

        /// <summary>
        /// Map Doctor → PublicProfileDto (chi tiết public).
        /// </summary>
        public static DoctorPublicProfileDto ToPublicProfileDto(
            Doctor doctor,
            string accountFullName,
            Department? department)
        {
            return new DoctorPublicProfileDto(
                Id: doctor.Id,
                Slug: doctor.Slug,
                FullName: accountFullName,
                Avatar: doctor.Avatar,
                Title: doctor.Title,
                Specialization: doctor.Specialization,
                Biography: doctor.Biography,
                YearsOfExperience: doctor.YearsOfExperience,
                Languages: doctor.Languages,
                ConsultationFee: doctor.ConsultationFee,
                Gender: doctor.Gender,
                Age: CalculateAge(doctor.DateOfBirth),
                Description: doctor.Description,
                DepartmentId: department?.Id,
                DepartmentSlug: department?.Slug,
                DepartmentName: department?.Name);
        }

        /// <summary>
        /// Map Doctor → PublicListDto (danh sách public).
        /// </summary>
        public static DoctorPublicListDto ToPublicListDto(
            Doctor doctor,
            string accountFullName,
            Department? department)
        {
            return new DoctorPublicListDto(
                Id: doctor.Id,
                Slug: doctor.Slug,
                FullName: accountFullName,
                Avatar: doctor.Avatar,
                Title: doctor.Title,
                Specialization: doctor.Specialization,
                YearsOfExperience: doctor.YearsOfExperience,
                ConsultationFee: doctor.ConsultationFee,
                Gender: doctor.Gender,
                DepartmentId: department?.Id,
                DepartmentSlug: department?.Slug,
                DepartmentName: department?.Name);
        }

        /// <summary>
        /// Map Doctor → LookupDto (dropdown).
        /// </summary>
        public static DoctorLookupDto ToLookupDto(
            Doctor doctor,
            string accountFullName)
        {
            return new DoctorLookupDto(
                Id: doctor.Id,
                FullName: accountFullName,
                Avatar: doctor.Avatar,
                Title: doctor.Title,
                Specialization: doctor.Specialization,
                DepartmentId: doctor.DepartmentId,
                SortOrder: doctor.SortOrder);
        }

        // ==================== ADMIN ====================

        /// <summary>
        /// Map Doctor → AdminListDto.
        /// </summary>
        public static DoctorAdminListDto ToAdminListDto(
            Doctor doctor,
            string accountFullName,
            string? departmentName)
        {
            return new DoctorAdminListDto(
                Id: doctor.Id,
                DoctorCode: doctor.DoctorCode,
                Slug: doctor.Slug,
                FullName: accountFullName,
                Avatar: doctor.Avatar,
                Title: doctor.Title,
                Specialization: doctor.Specialization,
                ConsultationFee: doctor.ConsultationFee,
                IsActive: doctor.IsActive,
                IsAvailableForOnline: doctor.IsAvailableForOnline,
                SortOrder: doctor.SortOrder,
                DepartmentId: doctor.DepartmentId,
                DepartmentName: departmentName,
                IsLicenseExpiringSoon: doctor.IsLicenseExpiringSoon());
        }

        /// <summary>
        /// Map Doctor → AdminDetailDto.
        /// </summary>
        public static DoctorAdminDetailDto ToAdminDetailDto(
            Doctor doctor,
            Account account,
            Department? department)
        {
            return new DoctorAdminDetailDto(
                Id: doctor.Id,
                DoctorCode: doctor.DoctorCode,
                Slug: doctor.Slug,
                AccountId: doctor.AccountId,
                FullName: account.FullName,
                Email: account.Email,
                PhoneNumber: account.PhoneNumber,

                Title: doctor.Title,
                Specialization: doctor.Specialization,
                YearsOfExperience: doctor.YearsOfExperience,
                Biography: doctor.Biography,
                Languages: doctor.Languages,

                Avatar: doctor.Avatar,
                FullAddress: doctor.FullAddress,
                Gender: doctor.Gender,
                DateOfBirth: doctor.DateOfBirth,
                IdentityNumber: doctor.IdentityNumber,

                LicenseNumber: doctor.LicenseNumber,
                LicenseIssueDate: doctor.LicenseIssueDate,
                LicenseExpiryDate: doctor.LicenseExpiryDate,
                LicenseIssuedBy: doctor.LicenseIssuedBy,

                ConsultationFee: doctor.ConsultationFee,
                MaxPatientsPerDay: doctor.MaxPatientsPerDay,
                JoinDate: doctor.JoinDate,

                Description: doctor.Description,

                IsAvailableForOnline: doctor.IsAvailableForOnline,
                IsActive: doctor.IsActive,
                SortOrder: doctor.SortOrder,

                DepartmentId: doctor.DepartmentId,
                DepartmentName: department?.Name,
                DepartmentSlug: department?.Slug,

                CreatedDate: doctor.CreatedDate,
                UpdatedDate: doctor.UpdatedDate);
        }

        // ==================== SELF ====================

        /// <summary>
        /// Map Doctor → SelfProfileDto (Doctor tự xem).
        /// </summary>
        public static DoctorSelfProfileDto ToSelfProfileDto(
            Doctor doctor,
            Account account,
            Department? department)
        {
            return new DoctorSelfProfileDto(
                Id: doctor.Id,
                DoctorCode: doctor.DoctorCode,
                Slug: doctor.Slug,
                AccountId: doctor.AccountId,
                FullName: account.FullName,
                Email: account.Email,
                PhoneNumber: account.PhoneNumber,

                Title: doctor.Title,
                Specialization: doctor.Specialization,
                YearsOfExperience: doctor.YearsOfExperience,
                Biography: doctor.Biography,
                Languages: doctor.Languages,

                Avatar: doctor.Avatar,
                FullAddress: doctor.FullAddress,
                Gender: doctor.Gender,
                DateOfBirth: doctor.DateOfBirth,
                IdentityNumber: doctor.IdentityNumber,

                LicenseNumber: doctor.LicenseNumber,
                LicenseIssueDate: doctor.LicenseIssueDate,
                LicenseExpiryDate: doctor.LicenseExpiryDate,
                LicenseIssuedBy: doctor.LicenseIssuedBy,
                IsLicenseExpiringSoon: doctor.IsLicenseExpiringSoon(),

                ConsultationFee: doctor.ConsultationFee,
                MaxPatientsPerDay: doctor.MaxPatientsPerDay,
                JoinDate: doctor.JoinDate,

                Description: doctor.Description,

                IsAvailableForOnline: doctor.IsAvailableForOnline,
                IsActive: doctor.IsActive,

                DepartmentId: doctor.DepartmentId,
                DepartmentName: department?.Name,
                DepartmentSlug: department?.Slug);
        }

        // ==================== HELPERS ====================

        /// <summary>
        /// Tính tuổi từ ngày sinh. Trả null nếu DOB null.
        /// </summary>
        private static int? CalculateAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
                return null;

            var today = DateTime.UtcNow.Date;
            var age = today.Year - dateOfBirth.Value.Year;

            if (dateOfBirth.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
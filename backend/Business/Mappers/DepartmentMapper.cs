using BaseClinic.Business.Models.Department;
using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Mappers
{
    /// <summary>
    /// Helper chuyển Department entity sang các DTO.
    /// Tách ra để tái sử dụng ở nhiều Query.
    /// 
    /// LƯU Ý: Department KHÔNG có navigation tới Doctor.
    /// → Query Handler phải tự load doctors và truyền vào mapper.
    /// </summary>
    public static class DepartmentMapper
    {
        /// <summary>
        /// Map sang DTO cho danh sách Admin.
        /// Cần truyền doctorCount + activeDoctorCount (query handler tính).
        /// </summary>
        public static DepartmentDetailDto ToDetailDto(
            Department dept,
            int doctorCount,
            int activeDoctorCount)
        {
            return new DepartmentDetailDto(
                Id: dept.Id,
                Name: dept.Name,
                DepartmentCode: dept.DepartmentCode,
                ImageUrl: dept.ImageUrl,
                ShortDescription: dept.ShortDescription,
                FullDescription: dept.FullDescription,
                Location: dept.Location,
                PhoneNumber: dept.PhoneNumber,

                Is24Hours: dept.Is24Hours,
                OpeningTime: dept.OpeningTime,
                ClosingTime: dept.ClosingTime,

                SortOrder: dept.SortOrder,
                IsActive: dept.IsActive,

                DoctorCount: doctorCount,
                ActiveDoctorCount: activeDoctorCount,

                CreatedDate: dept.CreatedDate,
                UpdatedDate: dept.UpdatedDate
            );
        }

        /// <summary>
        /// Map sang DTO chi tiết cho Admin.
        /// Bao gồm danh sách doctors (đã được query handler load trước).
        /// </summary>
        public static DepartmentAdminDetailDto ToAdminDetailDto(
            Department dept,
            List<Doctor> doctors,
            Dictionary<Guid, string> accountNamesById)
        {
            var doctorDtos = doctors
                .OrderBy(d => d.SortOrder)
                .Select(d => ToDoctorSummaryDto(d, accountNamesById))
                .ToList();

            return new DepartmentAdminDetailDto(
                Id: dept.Id,
                Name: dept.Name,
                DepartmentCode: dept.DepartmentCode,
                ImageUrl: dept.ImageUrl,
                ShortDescription: dept.ShortDescription,
                FullDescription: dept.FullDescription,
                Location: dept.Location,
                PhoneNumber: dept.PhoneNumber,

                Is24Hours: dept.Is24Hours,
                OpeningTime: dept.OpeningTime,
                ClosingTime: dept.ClosingTime,

                SortOrder: dept.SortOrder,
                IsActive: dept.IsActive,

                CreatedDate: dept.CreatedDate,
                UpdatedDate: dept.UpdatedDate,

                Doctors: doctorDtos
            );
        }

        /// <summary>
        /// Map sang DTO chi tiết cho Guest/Patient.
        /// Chỉ bao gồm doctors active.
        /// </summary>
        public static DepartmentPublicDetailDto ToPublicDetailDto(
            Department dept,
            List<Doctor> activeDoctors,
            Dictionary<Guid, string> accountNamesById)
        {
            var doctorDtos = activeDoctors
                .OrderBy(d => d.SortOrder)
                .Select(d => ToDoctorPublicDto(d, accountNamesById))
                .ToList();

            return new DepartmentPublicDetailDto(
                Id: dept.Id,
                Name: dept.Name,
                DepartmentCode: dept.DepartmentCode,
                ImageUrl: dept.ImageUrl,
                ShortDescription: dept.ShortDescription,
                FullDescription: dept.FullDescription,
                Location: dept.Location,
                PhoneNumber: dept.PhoneNumber,

                Is24Hours: dept.Is24Hours,
                OpeningTime: dept.OpeningTime,
                ClosingTime: dept.ClosingTime,

                Doctors: doctorDtos
            );
        }

        /// <summary>
        /// Map Department → LookupDto (cho dropdown).
        /// </summary>
        public static DepartmentLookupDto ToLookupDto(Department dept)
        {
            return new DepartmentLookupDto(
                Id: dept.Id,
                Name: dept.Name,
                DepartmentCode: dept.DepartmentCode,
                SortOrder: dept.SortOrder
            );
        }

        // ==================== DOCTOR HELPERS ====================

        /// <summary>
        /// Map Doctor → SummaryDto (cho Admin).
        /// </summary>
        private static DepartmentDoctorSummaryDto ToDoctorSummaryDto(
            Doctor doctor,
            Dictionary<Guid, string> accountNamesById)
        {
            accountNamesById.TryGetValue(doctor.AccountId, out var fullName);

            return new DepartmentDoctorSummaryDto(
                Id: doctor.Id,
                DoctorCode: doctor.DoctorCode,
                FullName: fullName ?? string.Empty,
                Avatar: doctor.Avatar,
                Title: doctor.Title,
                Specialization: doctor.Specialization,
                IsActive: doctor.IsActive,
                IsAvailableForOnline: doctor.IsAvailableForOnline,
                ConsultationFee: doctor.ConsultationFee,
                SortOrder: doctor.SortOrder
            );
        }

        /// <summary>
        /// Map Doctor → PublicDto (cho Guest/Patient).
        /// </summary>
        private static DepartmentDoctorPublicDto ToDoctorPublicDto(
            Doctor doctor,
            Dictionary<Guid, string> accountNamesById)
        {
            accountNamesById.TryGetValue(doctor.AccountId, out var fullName);

            return new DepartmentDoctorPublicDto(
                Id: doctor.Id,
                FullName: fullName ?? string.Empty,
                Avatar: doctor.Avatar,
                Title: doctor.Title,
                Specialization: doctor.Specialization,
                Biography: doctor.Biography,
                YearsOfExperience: doctor.YearsOfExperience,
                ConsultationFee: doctor.ConsultationFee
            );
        }
    }
}
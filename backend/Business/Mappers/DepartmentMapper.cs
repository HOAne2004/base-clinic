using BaseClinic.Business.Models.Department;
using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Mappers
{
    /// <summary>
    /// Helper chuyển Department entity sang các DTO.
    /// </summary>
    public static class DepartmentMapper
    {
        /// <summary>
        /// Map sang DepartmentAdminListDto (cho list admin).
        /// </summary>
        public static DepartmentAdminListDto ToAdminListDto(
            Department dept,
            int activeDoctorCount)
        {
            return new DepartmentAdminListDto(
                Id: dept.Id,
                Name: dept.Name,
                DepartmentCode: dept.DepartmentCode,
                PhoneNumber: dept.PhoneNumber,

                Is24Hours: dept.Is24Hours,
                OpeningTime: dept.OpeningTime,
                ClosingTime: dept.ClosingTime,

                IsActive: dept.IsActive,
                SortOrder: dept.SortOrder,

                ActiveDoctorCount: activeDoctorCount);
        }

        /// <summary>
        /// Map sang DepartmentAdminDetailDto (cho detail admin).
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

                Doctors: doctorDtos);
        }

        /// <summary>
        /// Map sang DepartmentPublicDetailDto (cho guest).
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

                Doctors: doctorDtos);
        }

        /// <summary>
        /// Map Department → LookupDto.
        /// </summary>
        public static DepartmentLookupDto ToLookupDto(Department dept)
        {
            return new DepartmentLookupDto(
                Id: dept.Id,
                Name: dept.Name,
                DepartmentCode: dept.DepartmentCode,
                SortOrder: dept.SortOrder);
        }

        /// <summary>
        /// Map sang DepartmentPublicListDto (cho landing page).
        /// </summary>
        public static DepartmentPublicListDto ToPublicListDto(
            Department dept,
            int activeDoctorCount)
        {
            return new DepartmentPublicListDto(
                Id: dept.Id,
                Name: dept.Name,
                ImageUrl: dept.ImageUrl,
                ShortDescription: dept.ShortDescription,
                Location: dept.Location,
                PhoneNumber: dept.PhoneNumber,
                IsActive: dept.IsActive,
                Is24Hours: dept.Is24Hours,
                OpeningTime: dept.OpeningTime,
                ClosingTime: dept.ClosingTime,
                ActiveDoctorCount: activeDoctorCount);
        }

        // ==================== DOCTOR HELPERS ====================

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
                SortOrder: doctor.SortOrder);
        }

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
                ConsultationFee: doctor.ConsultationFee);
        }
    }
}
using BaseClinic.Business.Models.Common;
using BaseClinic.Business.Models.Patient;
using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Mappers
{
    /// <summary>
    /// Helper chuyển Patient entity sang PatientDto.
    /// Tách ra để tái sử dụng ở nhiều Query/Command.
    /// </summary>
    public static class PatientMapper
    {
        public static PatientDto ToDto(Patient p)
        {
            AddressDto? addressDto = null;
            if (!p.Address.IsEmpty())
            {
                addressDto = new AddressDto(
                    Tinh: p.Address.Tinh,
                    Xa: p.Address.Xa,
                    Duong: p.Address.Duong,
                    SoNha: p.Address.SoNha);
            }

            return new PatientDto(
                Id: p.Id,
                PatientCode: p.PatientCode,
                FullName: p.FullName,
                AvatarUrl: p.AvatarUrl,
                Gender: p.Gender,
                DateOfBirth: p.DateOfBirth,
                IdentityNumber: p.IdentityNumber,
                HealthInsuranceNumber: p.HealthInsuranceNumber,
                AddressFullText: p.Address.IsEmpty() ? null : p.Address.FullText,
                Address: addressDto,
                AccountId: p.AccountId,
                PrimaryPatientId: p.PrimaryPatientId,
                PhoneNumber: p.Account?.PhoneNumber,
                Email: p.Account?.Email,
                CreatedDate: p.CreatedDate,
                UpdatedDate: p.UpdatedDate);
        }

        /// <summary>
        /// Phiên bản map cho danh sách (không cần Address chi tiết).
        /// </summary>
        public static PatientListItemDto ToListItemDto(Patient p)
        {
            return new PatientListItemDto(
                Id: p.Id,
                PatientCode: p.PatientCode,
                FullName: p.FullName,
                Gender: p.Gender,
                DateOfBirth: p.DateOfBirth,
                IdentityNumber: p.IdentityNumber,
                HealthInsuranceNumber: p.HealthInsuranceNumber,
                PhoneNumber: p.Account?.PhoneNumber,
                AddressFullText: p.Address.IsEmpty() ? null : p.Address.FullText);
        }
    }
}
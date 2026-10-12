using System.ComponentModel.DataAnnotations;
using BaseClinic.Business.Models.Common;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.Patient
{
    public class CreateDependentDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quan hệ không được để trống.")]
        public PatientRelationshipType RelationshipType { get; set; }

        public bool? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? IdentityNumber { get; set; }

        [StringLength(20)]
        public string? HealthInsuranceNumber { get; set; }

        public AddressDto? Address { get; set; }
    }
}
using BaseClinic.Domain.ValueObjects;

namespace BaseClinic.Business.Models.Patient
{
    public class UpdatePatientDto
    {
        public string FullName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public bool? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? IdentityNumber { get; set; }

        public string? HealthInsuranceNumber { get; set; }

        public Address? Address { get; set; }
    }
}
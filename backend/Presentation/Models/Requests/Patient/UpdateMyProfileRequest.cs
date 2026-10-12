using BaseClinic.Business.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace BaseClinic.Presentation.Models.Requests.Patient
{
    public class UpdateMyProfileRequest
    {
        public string? AvatarUrl { get; set; }

        public AddressDto? Address { get; set; }

    }
}
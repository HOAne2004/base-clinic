namespace BaseClinic.Presentation.Models.Requests.DoctorSchedule
{
    public class SetScheduleOnlineBookingRequest
    {
        [Required(ErrorMessage = "Enabled không được để trống.")]
        public bool Enabled { get; set; }
    }
}

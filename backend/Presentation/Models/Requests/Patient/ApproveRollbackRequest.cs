using BaseClinic.Business.Models.Patient;

namespace BaseClinic.Presentation.Models.Requests.Patient
{
    public class ApproveRollbackRequest
    {
        public string? ReviewNote { get; set; }

        public List<ConflictResolutionDto> ConflictResolutions { get; set; } = new();
    }
}
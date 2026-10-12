namespace BaseClinic.Business.Models.Patient
{
    public record CreateDependentResultDto(
         Guid DependentId,
         string PatientCode
     );
}

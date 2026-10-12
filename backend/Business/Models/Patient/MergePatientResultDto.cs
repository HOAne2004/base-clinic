namespace BaseClinic.Business.Models.Patient
{
    public record MergePatientResultDto(
        Guid SurvivorPatientId,
        string SurvivorPatientCode,
        Guid MergedPatientId,
        string MergedPatientCode,
        int AppointmentsMoved,
        int EncountersMoved,
        int DelegationsMoved,
        int DependentsMoved,
        bool AccountMoved
    );
}
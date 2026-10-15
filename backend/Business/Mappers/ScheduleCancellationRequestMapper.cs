using BaseClinic.Business.Models.DoctorSchedule;
using BaseClinic.Domain.Entities;

namespace BaseClinic.Business.Mappers
{
    public static class ScheduleCancellationRequestMapper
    {
        /// <summary>
        /// Map request → ListDto.
        /// </summary>
        public static ScheduleCancellationRequestDto ToDto(
            ScheduleCancellationRequest request,
            string requesterName,
            string? reviewerName,
            string doctorName,
            Department? department)
        {
            var schedule = request.DoctorSchedule;

            return new ScheduleCancellationRequestDto(
                Id: request.Id,
                DoctorScheduleId: schedule.Id,
                WorkDate: schedule.WorkDate,
                StartTime: schedule.StartTime,
                EndTime: schedule.EndTime,

                DoctorId: schedule.DoctorId,
                DoctorName: doctorName,
                DepartmentId: schedule.Doctor.DepartmentId,
                DepartmentName: department?.Name,

                RequestedByAccountId: request.RequestedByAccountId,
                RequesterName: requesterName,
                Reason: request.Reason,

                Status: request.Status,
                CreatedDate: request.CreatedDate,

                ReviewedByAccountId: request.ReviewedByAccountId,
                ReviewerName: reviewerName,
                ReviewedAt: request.ReviewedAt,
                ReviewNote: request.ReviewNote);
        }

        /// <summary>
        /// Map request → DetailDto (bao gồm full schedule).
        /// </summary>
        public static ScheduleCancellationRequestDetailDto ToDetailDto(
            ScheduleCancellationRequest request,
            string requesterName,
            string? reviewerName,
            DoctorScheduleListDto scheduleDto)
        {
            return new ScheduleCancellationRequestDetailDto(
                Id: request.Id,
                Status: request.Status,
                Reason: request.Reason,
                CreatedDate: request.CreatedDate,

                RequestedByAccountId: request.RequestedByAccountId,
                RequesterName: requesterName,

                ReviewedByAccountId: request.ReviewedByAccountId,
                ReviewerName: reviewerName,
                ReviewedAt: request.ReviewedAt,
                ReviewNote: request.ReviewNote,

                Schedule: scheduleDto);
        }
    }
}
using BaseClinic.Domain.Enums;

namespace BaseClinic.Business.Models.DoctorSchedule
{
    /// <summary>
    /// Filter cho danh sách ca làm việc.
    /// 
    /// SORT: WorkDate ASC → StartTime ASC.
    /// </summary>
    public class ScheduleSearchDto
    {
        public Guid? DoctorId { get; set; }
        public Guid? DepartmentId { get; set; }

        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }

        public DoctorScheduleStatus? Status { get; set; }
        public bool? OnlineBookingEnabled { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
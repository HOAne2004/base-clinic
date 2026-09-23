using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Domain.Entities
{
    public class QueueEntry : BaseEntity
    {
        public Guid QueueId { get; private set; }
        public Guid EncounterId { get; private set; }
        public int QueueNumber { get; private set; }
        public bool IsPriority { get; private set; }
        public DateTime CheckInAt { get; private set; }
        public DateTime? CallAt { get; private set; }
        public DateTime? CompleteAt { get; private set; }
        public QueueEntryStatus Status { get; private set; }

        private QueueEntry() { }

        internal QueueEntry(Guid queueId, Guid encounterId, int queueNumber, bool isPriority, DateTime checkInAt)
        {
            QueueId = queueId;
            EncounterId = encounterId;
            QueueNumber = queueNumber;
            IsPriority = isPriority;
            CheckInAt = checkInAt;
            Status = QueueEntryStatus.Waiting;
        }

        internal void MarkAsCalled(DateTime callAt)
        {
            if (Status != QueueEntryStatus.Waiting && Status != QueueEntryStatus.Skipped)
                throw new InvalidOperationException("Chỉ có thể gọi bệnh nhân đang chờ hoặc đã bị bỏ qua.");

            CallAt = callAt;
            Status = QueueEntryStatus.Called;
        }

        internal void MarkAsInProgress()
        {
            if (Status != QueueEntryStatus.Called)
                throw new InvalidOperationException("Chỉ có thể chuyển sang khám (InProgress) khi bệnh nhân đã được gọi tên.");
            Status = QueueEntryStatus.InProgress;
        }

        internal void MarkAsCompleted(DateTime completeAt)
        {
            if (Status != QueueEntryStatus.InProgress)
                throw new InvalidOperationException("Trạng thái phải là InProgress để hoàn thành.");
            CompleteAt = completeAt;
            Status = QueueEntryStatus.Completed;
        }

        internal void Skip()
        {
            if (Status != QueueEntryStatus.Called)
                throw new InvalidOperationException("Chỉ có thể bỏ qua (Skip) bệnh nhân đã được gọi nhưng không xuất hiện.");
            Status = QueueEntryStatus.Skipped;
        }

        internal void Cancel()
        {
            if (Status is QueueEntryStatus.Completed or QueueEntryStatus.InProgress)
                throw new InvalidOperationException("Không thể hủy lượt xếp hàng đang khám hoặc đã hoàn thành.");
            Status = QueueEntryStatus.Cancelled;
        }
    }
}
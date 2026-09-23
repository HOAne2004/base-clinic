using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;

namespace BaseClinic.Domain.Entities
{
    public class Queue : AggregateRoot
    {
        public Guid DepartmentId { get; private set; }
        public string QueueCode { get; private set; } = null!;
        public DateTime QueueDate { get; private set; }
        public QueueType Type { get; private set; }
        public QueueStatus Status { get; private set; }

        private readonly List<QueueEntry> _queueEntries = new();
        public IReadOnlyCollection<QueueEntry> QueueEntries => _queueEntries.AsReadOnly();

        private Queue() { }

        public Queue(Guid departmentId, string queueCode, DateTime queueDate, QueueType type)
        {
            if (string.IsNullOrWhiteSpace(queueCode))
                throw new ArgumentException("Mã hàng đợi không được để trống.");

            DepartmentId = departmentId;
            QueueCode = queueCode;
            QueueDate = queueDate.Date;
            Type = type;
            Status = QueueStatus.Open;
        }

        // --- QUẢN LÝ TRẠNG THÁI HÀNG ĐỢI ---
        public void Pause()
        {
            if (Status == QueueStatus.Closed)
                throw new InvalidOperationException("Không thể tạm dừng hàng đợi đã đóng.");
            Status = QueueStatus.Paused;
        }

        public void Resume()
        {
            if (Status == QueueStatus.Closed)
                throw new InvalidOperationException("Không thể mở lại hàng đợi đã đóng.");
            Status = QueueStatus.Open;
        }

        public void Close()
        {
            Status = QueueStatus.Closed;
        }

        // --- ĐIỀU PHỐI BỆNH NHÂN XẾP HÀNG ---
        public QueueEntry Enqueue(Guid encounterId, bool isPriority, DateTime checkInAt)
        {
            if (Status != QueueStatus.Open)
                throw new InvalidOperationException("Không thể nhận thêm bệnh nhân vào hàng đợi không mở.");

            // Sinh số thứ tự tự động tăng
            var nextNumber = _queueEntries.Any() ? _queueEntries.Max(x => x.QueueNumber) + 1 : 1;

            var entry = new QueueEntry(Id, encounterId, nextNumber, isPriority, checkInAt);
            _queueEntries.Add(entry);

            return entry;
        }

        public void CallPatient(Guid queueEntryId, DateTime callTime)
        {
            var entry = GetEntry(queueEntryId);
            entry.MarkAsCalled(callTime);
        }

        public void StartEncounter(Guid queueEntryId)
        {
            var entry = GetEntry(queueEntryId);
            entry.MarkAsInProgress();
        }

        public void CompleteEncounter(Guid queueEntryId, DateTime completeTime)
        {
            var entry = GetEntry(queueEntryId);
            entry.MarkAsCompleted(completeTime);
        }

        public void SkipPatient(Guid queueEntryId)
        {
            var entry = GetEntry(queueEntryId);
            entry.Skip();
        }

        public void CancelEntry(Guid queueEntryId)
        {
            var entry = GetEntry(queueEntryId);
            entry.Cancel();
        }

        private QueueEntry GetEntry(Guid entryId)
        {
            var entry = _queueEntries.FirstOrDefault(x => x.Id == entryId);
            if (entry == null)
                throw new ArgumentException("Không tìm thấy lượt xếp hàng này trong hệ thống.");
            return entry;
        }
    }
}
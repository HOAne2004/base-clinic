using BaseClinic.Domain.Common;

namespace BaseClinic.Domain.Entities
{
    public class PrescriptionItem : BaseEntity
    {
        public Guid PrescriptionId { get; private set; }
        public Guid MedicineId { get; private set; }

        // Snapshot lưu cứng tên thuốc tại thời điểm kê đơn (Immutable)
        public string MedicineNameSnapShot { get; private set; } = string.Empty;

        public string Unit { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public string Instructions { get; private set; } = string.Empty;
        public int DurationDays { get; private set; }

        private PrescriptionItem() { }

        internal PrescriptionItem(Guid prescriptionId, Guid medicineId, string medicineNameSnapShot, string unit, int quantity, string instructions, int durationDays)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng thuốc phải lớn hơn 0.");
            if (durationDays <= 0) throw new ArgumentException("Số ngày sử dụng thuốc phải lớn hơn 0.");
            if (string.IsNullOrWhiteSpace(medicineNameSnapShot)) throw new ArgumentException("Tên thuốc không được để trống.");
            if (string.IsNullOrWhiteSpace(unit)) throw new ArgumentException("Đơn vị tính không được để trống.");

            PrescriptionId = prescriptionId;
            MedicineId = medicineId;
            MedicineNameSnapShot = medicineNameSnapShot.Trim();
            Unit = unit.Trim();
            Quantity = quantity;
            Instructions = instructions?.Trim() ?? string.Empty;
            DurationDays = durationDays;
        }

        internal void UpdateInstructions(int quantity, string instructions, int durationDays)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng thuốc phải lớn hơn 0.");
            if (durationDays <= 0) throw new ArgumentException("Số ngày sử dụng thuốc phải lớn hơn 0.");

            Quantity = quantity;
            Instructions = instructions?.Trim() ?? string.Empty;
            DurationDays = durationDays;
        }
    }
}
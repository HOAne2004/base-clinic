using BaseClinic.Domain.Common;
using BaseClinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseClinic.Domain.Entities
{
    public class Prescription : AggregateRoot
    {
        public Guid ConsultationId { get; private set; }
        public string PrescriptionCode { get; private set; } = string.Empty;
        public DateTime IssuedDate { get; private set; }
        public DateTime ValidUntil { get; private set; }
        public string? Notes { get; private set; }
        public PrescriptionStatus Status { get; private set; }

        private readonly List<PrescriptionItem> _prescriptionItems = new();
        public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _prescriptionItems.AsReadOnly();

        private Prescription() { }

        public Prescription(Guid consultationId, string prescriptionCode, DateTime issuedDate, DateTime validUntil, string? notes)
        {
            if (string.IsNullOrWhiteSpace(prescriptionCode))
                throw new ArgumentException("Mã đơn thuốc không được để trống.");
            if (validUntil <= issuedDate)
                throw new ArgumentException("Hạn sử dụng của đơn thuốc phải sau ngày kê đơn.");

            ConsultationId = consultationId;
            PrescriptionCode = prescriptionCode.Trim();
            IssuedDate = issuedDate;
            ValidUntil = validUntil;
            Notes = notes?.Trim();

            // Trạng thái mặc định khi bác sĩ vừa kê xong là Pending
            Status = PrescriptionStatus.Pending;
        }

        // --- CÁC HÀNH VI QUẢN LÝ TRẠNG THÁI (STATE MACHINE) ---
        public void Approve()
        {
            if (Status != PrescriptionStatus.Pending)
                throw new InvalidOperationException("Chỉ có thể duyệt đơn thuốc đang chờ xử lý (Pending).");
            Status = PrescriptionStatus.Approved;
        }

        public void Dispense()
        {
            if (Status != PrescriptionStatus.Approved)
                throw new InvalidOperationException("Chỉ có thể phát thuốc cho đơn đã được duyệt (Approved).");
            Status = PrescriptionStatus.Active;
        }

        public void Cancel()
        {
            // Nếu đã phát thuốc (Active) hoặc hết hạn (Expired) thì không cho phép hủy ngang
            if (Status is PrescriptionStatus.Active or PrescriptionStatus.Expired)
                throw new InvalidOperationException("Không thể hủy đơn thuốc đã phát cho bệnh nhân hoặc đã hết hạn.");

            Status = PrescriptionStatus.Cancelled;
        }

        public void Expire()
        {
            Status = PrescriptionStatus.Expired;
        }

        public void ExtendValidity(DateTime newValidUntil)
        {
            if (newValidUntil <= IssuedDate)
                throw new ArgumentException("Hạn sử dụng mới phải sau ngày kê đơn.");
            ValidUntil = newValidUntil;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes?.Trim();
        }

        // --- CÁC HÀNH VI QUẢN LÝ CHI TIẾT THUỐC ---
        public void AddMedicine(Guid medicineId, string medicineName, string unit, int quantity, string instructions, int durationDays)
        {
            // Bác sĩ chỉ được phép chỉnh sửa đơn thuốc khi nó chưa được duyệt
            if (Status != PrescriptionStatus.Pending)
                throw new InvalidOperationException("Chỉ có thể kê thêm thuốc vào đơn đang chờ xử lý.");

            var existingItem = _prescriptionItems.FirstOrDefault(x => x.MedicineId == medicineId);
            if (existingItem != null)
                throw new InvalidOperationException("Thuốc này đã có trong đơn. Vui lòng sử dụng chức năng cập nhật liều lượng.");

            var item = new PrescriptionItem(Id, medicineId, medicineName, unit, quantity, instructions, durationDays);
            _prescriptionItems.Add(item);
        }

        public void UpdateMedicineInstruction(Guid medicineId, int quantity, string instructions, int durationDays)
        {
            if (Status != PrescriptionStatus.Pending)
                throw new InvalidOperationException("Chỉ có thể thay đổi liều lượng khi đơn đang chờ xử lý.");

            var item = _prescriptionItems.FirstOrDefault(x => x.MedicineId == medicineId);
            if (item == null)
                throw new ArgumentException("Không tìm thấy loại thuốc này trong đơn.");

            item.UpdateInstructions(quantity, instructions, durationDays);
        }

        public void RemoveMedicine(Guid medicineId)
        {
            if (Status != PrescriptionStatus.Pending)
                throw new InvalidOperationException("Chỉ có thể xóa thuốc khỏi đơn đang chờ xử lý.");

            var item = _prescriptionItems.FirstOrDefault(x => x.MedicineId == medicineId);
            if (item != null)
            {
                _prescriptionItems.Remove(item);
            }
        }
    }
}
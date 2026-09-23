using BaseClinic.Domain.Common;

namespace BaseClinic.Domain.Entities
{
    public class Medicine : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string MedicineCode { get; private set; } = string.Empty;
        public string? ImageUrl { get; private set; }
        public string Unit { get; private set; } = string.Empty;
        public string? ActiveIngredient { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsPrescriptionRequired { get; private set; }

        private Medicine() { }

        public Medicine(string medicineCode, string name, string unit, string? imageUrl = null, string? activeIngredient = null, string? description = null, bool isPrescriptionRequired = false)
        {
            if (string.IsNullOrWhiteSpace(medicineCode))
                throw new ArgumentException("Mã thuốc không được để trống.");

            MedicineCode = medicineCode.Trim(); // Bất biến sau khởi tạo
            IsActive = true; // Mặc định kích hoạt khi tạo mới

            UpdateDetails(name, unit, imageUrl, activeIngredient, description, isPrescriptionRequired);
        }

        public void UpdateDetails(string name, string unit, string? imageUrl, string? activeIngredient, string? description, bool isPrescriptionRequired)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên thuốc không được để trống.");
            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Đơn vị tính không được để trống.");

            Name = name.Trim();
            Unit = unit.Trim();
            ImageUrl = imageUrl?.Trim();
            ActiveIngredient = activeIngredient?.Trim();
            Description = description?.Trim();
            IsPrescriptionRequired = isPrescriptionRequired;
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;
    }
}
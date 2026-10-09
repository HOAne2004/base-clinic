namespace BaseClinic.Domain.Enums
{
    public enum PatientRelationshipType
    {
        Self = 0,           // Chính bệnh nhân (khi patient tự đặt lịch)
        Wife = 1,
        Husband = 2,
        Child = 3,
        Parent = 4,
        Sibling = 5,        // Anh/Chị/Em
        Grandparent = 6,    // Ông/Bà
        Grandchild = 7,     // Cháu
        Relative = 8,       // Họ hàng khác
        Guardian = 9,       // Người giám hộ
        Other = 99          // Khác
    }
}
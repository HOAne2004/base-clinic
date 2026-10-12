using BaseClinic.Domain.Enums;

namespace BaseClinic.Domain.Extensions
{
    /// <summary>
    /// Extension methods cho PatientRelationshipType.
    /// </summary>
    public static class RelationshipTypeExtensions
    {
        /// <summary>
        /// Đảo ngược quan hệ.
        /// 
        /// Ví dụ:
        /// - B là Child của A → A là Parent của B.
        /// - A là Wife của B → B là Husband của A.
        /// 
        /// Dùng khi:
        /// - Patient B có RelationshipType = Child (B là con của A).
        /// - Tạo PatientDelegation cho A → cần RelationshipType = Parent (A là mẹ của B).
        /// </summary>
        public static PatientRelationshipType Invert(this PatientRelationshipType type)
        {
            return type switch
            {
                PatientRelationshipType.Wife => PatientRelationshipType.Husband,
                PatientRelationshipType.Husband => PatientRelationshipType.Wife,
                PatientRelationshipType.Child => PatientRelationshipType.Parent,
                PatientRelationshipType.Parent => PatientRelationshipType.Child,
                PatientRelationshipType.Sibling => PatientRelationshipType.Sibling,
                PatientRelationshipType.Grandparent => PatientRelationshipType.Grandchild,
                PatientRelationshipType.Grandchild => PatientRelationshipType.Grandparent,
                PatientRelationshipType.Guardian => PatientRelationshipType.Guardian, // 2 chiều
                PatientRelationshipType.Relative => PatientRelationshipType.Relative,
                PatientRelationshipType.Other => PatientRelationshipType.Other,
                PatientRelationshipType.Self => PatientRelationshipType.Self,
                _ => PatientRelationshipType.Other
            };
        }
    }
}
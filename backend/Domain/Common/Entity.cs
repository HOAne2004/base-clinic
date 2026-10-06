using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseClinic.Domain.Common
{
    [PrimaryKey(nameof(Id))]
    public class Entity : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        // DatabaseGenerated chỉ định ai chịu trách nhiệm sinh giá trị cho cột
        // Identity: DB tự sinh khi insert
        // Computed: DB tự tính mỗi khi insert/update
        // None: Applicaiton tự gán
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}

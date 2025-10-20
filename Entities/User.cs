

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class User
    {

        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //[Required]
        //[MaxLength(100)]
        public string Name { get; set; }
        //[Required]
        //[MaxLength(100)]
        public string Email { get; set; }

        // Добавляем поле времени создания
        //[Column("creation_date")]
        public DateTime CreatedAt { get; set; }

        // Опционально: время последнего обновления
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}

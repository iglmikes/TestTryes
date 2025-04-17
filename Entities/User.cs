

namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // Добавляем поле времени создания
        public DateTime CreatedAt { get; set; }

        // Опционально: время последнего обновления
        public DateTime? UpdatedAt { get; set; }
    }
}

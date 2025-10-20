

namespace Entities
{
    public class Order
    {

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}

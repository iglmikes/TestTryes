

namespace Entities
{
    public class Order
    {

        public int Id { get; set; }


        public int UserId { get; set; }
        

        public string Email { get; set; }
        public object User { get; set; }
    }
}

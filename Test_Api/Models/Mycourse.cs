using System.ComponentModel.DataAnnotations.Schema;

namespace Test_Api.Models
{
    [Table("Mycourse")]
    public class Mycourse
    {
        public int Id { get; set; }
        public int orderId { get; set; }
        public Order Order { get; set; }
        public int courseId { get; set; }
        public Course Course { get; set; }
        public decimal Price { get; set; }
    }
}

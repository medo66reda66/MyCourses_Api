using System.ComponentModel.DataAnnotations.Schema;
using Test_Api.Models;

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
        public string? ApplicationuserId {  get; set; }
        public ApplicationUser applicationUser { get; set; }
        public DateTime? BuyAt { get; set; } = DateTime.Now;
    }
}

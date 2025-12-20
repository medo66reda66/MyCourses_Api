using Microsoft.EntityFrameworkCore;

namespace Test_Api.Models
{
    [PrimaryKey(nameof(Courseid),nameof(ApplicationUserId))]
    public class Carts
    {
        public decimal Price { get; set; } 
        public int Courseid { get; set; }
        public Course Courses { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUsers { get; set; } = null!;

    }
}

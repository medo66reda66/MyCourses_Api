using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class promotionRequest
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public decimal DiscountPercentage { get; set; }
        [Required]
        public string code { get; set; } 
        //public DateTime StartDate { get; set; }= DateTime.UtcNow;
        //public DateTime EndDate { get; set; }= DateTime.UtcNow.AddMonths(1);
        [Required]
        public bool IsActive { get; set; }

    }
}

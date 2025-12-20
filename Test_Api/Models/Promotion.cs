namespace Test_Api.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }
    }
}

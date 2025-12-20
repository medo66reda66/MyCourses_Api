using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class CreateCourseRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public IFormFile Image { get; set; }= default!;
        public List<IFormFile>? Videos { get; set; }
        public DateTime? Start { get; set; }= DateTime.UtcNow;
        public DateTime? End { get; set; }= DateTime.UtcNow.AddMonths(1);
        public int? Discount { get; set; }= 0;
        [Required]
        public string language { get; set; }
        [Required]
        public int totalstudents { get; set; }= 0;
        [Required]
        public int QuantitylESONS { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int InstructorId { get; set; }
        [Required]
        public bool Status { get; set; }
        

    }
}

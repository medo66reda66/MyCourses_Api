using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class EditCourseRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IFormFile? NewImage { get; set; }
        public List<IFormFile>? Videos { get; set; }
        public DateTime Start { get; set; }= DateTime.UtcNow;
        public DateTime End { get; set; }
        public int? Discount { get; set; }= 0;
        public string language { get; set; }
        [Required]
        public int totalstudents { get; set; } = 0;
        [Required]
        public int QuantitylESONS { get; set; }
        public int CategoryId { get; set; }
        public int InstructorId { get; set; }
        public bool Status { get; set; }
    }
}

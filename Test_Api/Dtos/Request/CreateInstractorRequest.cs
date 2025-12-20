using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class CreateInstractorRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int Age { get; set; }
        public IFormFile? Image { get; set; }
        public string? ProfileUrl { get; set; } 

    }
}

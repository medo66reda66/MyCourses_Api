namespace Test_Api.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string? Image { get; set; } = string.Empty;
        public string? ProfileUrl { get; set; }
        public int Rating { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
}

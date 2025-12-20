using System.ComponentModel.DataAnnotations.Schema;

namespace Test_Api.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime start { get; set; } = DateTime.Now;
        public DateTime end { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public int? Discount { get; set; }
        public string language { get; set; }
        public int totalstudents { get; set; }
        [Column("QuantityLesons")]
        public int QuantityLesons { get; set; }
        public bool Status { get; set; }
        public int? Rat { get; set; }
        public int Instructorid { get; set; }
        public Instructor Instructors { get; set; } = null!;
        public List<CourseVideos> CourseVideos { get; set; }
        public int Categorysid { get; set; }
        public Category Categorys { get; set; } = null!;

    }
}

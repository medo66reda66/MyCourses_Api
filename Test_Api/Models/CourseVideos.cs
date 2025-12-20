using System.ComponentModel.DataAnnotations.Schema;

namespace Test_Api.Models
{
    [Table("CourseVideos")]
    public class CourseVideos
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Courses { get; set; } = null!;
        [Column("Video")]
        public string Video { get; set; }= string.Empty;
    }
}

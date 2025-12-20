using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test_Api.Models;

namespace Test_Api.Datebase.Entitytypeconficration
{
    public class CoursesSupImgEntitytypeconficratuon : IEntityTypeConfiguration<CourseVideos>
    {
        public void Configure(EntityTypeBuilder<CourseVideos> builder)
        {

            builder.HasKey(c => new { c.CourseId, c.Video });

        }
    }
}

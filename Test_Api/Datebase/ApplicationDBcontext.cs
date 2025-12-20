using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test_Api.Datebase.Entitytypeconficration;
using Test_Api.Models;

namespace Test_Api.Datebase
{
    public class ApplicationDBcontext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> dbContextOptions): base(dbContextOptions)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<CourseVideos> CourseVideos { get; set; }
        public DbSet<ApplicationuserOTP> ApplicationuserOTPs { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<Promotion> promotions { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Mycourse> Mycourses { get; set; }
        
     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial catalog= CourseApi_$; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoursesSupImgEntitytypeconficratuon).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
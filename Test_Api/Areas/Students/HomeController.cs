using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Dtos.Request;
using Test_Api.Dtos.Respons;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;

namespace Test_Api.Areas.Students
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Students")]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDBcontext _context ;
        private readonly IRepository<Course> _courserepository;
        public HomeController(ApplicationDBcontext context, IRepository<Course> courserepository)
        {
            _context = context;
            _courserepository = courserepository;
        }

        [HttpPost("GetAllCourses/{page}")]
        public async Task<IActionResult> GetAllCourses(FilterdataRequest filterdataRequest,CancellationToken cancellationToken,int page=1,int pagesize = 12)
        {
            var courses = await _courserepository.GetAllAsync(tracked: false, includes: [e => e.Categorys, e => e.Instructors], cancellationToken: cancellationToken);
            //_context.Courses.Include(e=>e.Categorys).Include(e=>e.Instructors).Include(e=>e.CourseSupImgs).AsQueryable();

           filterdataRespons filterdataRespons = new filterdataRespons();

            if (filterdataRequest.name is not null)
            {
                courses = courses.Where(e => e.Name.Contains(filterdataRequest.name));
                filterdataRespons.name = filterdataRequest.name;
            }
            if (filterdataRequest.minprice is not null)
            {
                courses = courses.Where(e => e.Price - e.Price * (e.Discount / 100) >= filterdataRequest.minprice);
                filterdataRespons.minprice = filterdataRequest.minprice;
            }
            if (filterdataRequest.maxprice is not null)
            {
                courses = courses.Where(e => e.Price - e.Price * (e.Discount / 100) <= filterdataRequest.maxprice);
                filterdataRespons.maxprice = filterdataRequest.maxprice;
            }
            if (filterdataRequest.catedoryid is not null)
            {
                courses = courses.Where(e => e.Categorysid == filterdataRequest.catedoryid);
                filterdataRespons.categoryid = filterdataRequest.catedoryid;
            }
            if (filterdataRequest.instractorid is not null)
            {
                courses = courses.Where(e => e.Instructorid == filterdataRequest.instractorid);
                filterdataRespons.inctractorid = filterdataRequest.instractorid;
            }
            //pagination
            paginationRespons paginationRespons = new paginationRespons();

            paginationRespons.Totalcount = courses.Count();//20
            paginationRespons.CurrentPage = page;
            paginationRespons.TotalPages = (int)Math.Ceiling((double)paginationRespons.Totalcount / pagesize);//3
            courses = courses.Skip((page - 1) * pagesize).Take(pagesize);//8

            return Ok(new {
               filter = filterdataRespons,
               pagination= paginationRespons, 
                data = courses.ToList() 
            });
        }

        [HttpGet("CoursesFree/{id}")]
        public async Task<IActionResult> CoursesFree(int id,CancellationToken cancellationToken)
        {
            var courses = await _courserepository.GetOneAsync(e => e.Id == id, tracked: false, includes: [e => e.Categorys, e => e.Instructors]);

            var freeCourses = (await _courserepository.GetAllAsync(
                expression: e => e.Name.Contains(courses.Name) 
                && e.Id != courses.Id 
                && e.Price - e.Price * (e.Discount / 100) == 0 
                && e.Categorysid == courses.Categorysid,
                includes: [e => e.Categorys, e => e.Instructors],
                tracked: false,
                cancellationToken)).Skip(0).Take(8);
                //_context.Courses.Where(e => e.Name.Contains(courses.Name) && e.Id != courses.Id && e.Price - e.Price * (e.Discount / 100) == 0 && e.Categorysid == id).Include(e => e.Categorys)
                //.Include(e => e.Instructors)
                //.Include(e => e.CourseSupImgs).Skip(0).Take(8);

            return Ok(new
            {
                course = courses,
                freecourses = freeCourses.ToList()
            });
        }

        [HttpGet("CoursesDiscount/{id}")]
        public async Task<IActionResult> CoursesDiscount(int id,CancellationToken cancellationToken)
        {
            var courses = await _courserepository.GetOneAsync(e => e.Id == id, tracked: false, includes: [e => e.Categorys, e => e.Instructors],cancellationToken:cancellationToken);
            //_context.Courses.Include(e => e.Categorys)
            //    .Include(e => e.Instructors)
            //    .Include(e => e.CourseSupImgs)
            //    .FirstOrDefault(e => e.Id == id);
               
            if (courses is null)
            {
                return NotFound(new
                {
                    Message = "Course not found"
                });
            }

            var discountCourses =(await _courserepository.GetAllAsync(expression: e => e.Name.Contains(courses.Name) && e.Id != courses.Id && e.Discount > 0,
                includes: [e => e.Categorys],
                tracked: false, cancellationToken: cancellationToken
                )).Skip(0).Take(8);

            //_context.Courses.Where(e => e.Name.Contains(courses.Name) && e.Id != courses.Id && e.Discount > 0).Include(e => e.Categorys)
            //    .Include(e => e.Instructors)
            //    .Include(e => e.CourseSupImgs).Skip(0).Take(8);

            return Ok(new
            {
                course = courses,
                discountcourses = discountCourses.ToList()
            });
        }
        [HttpGet("CourseRecomend/{id}")]
        public async Task<IActionResult> CourseRecomend(int id,CancellationToken cancellationToken)
        {
            var courses =await _courserepository.GetOneAsync(e => e.Id == id, tracked: false, includes: [e => e.Categorys, e => e.Instructors], cancellationToken: cancellationToken);
            //_context.Courses.Include(e => e.Categorys)
            //    .Include(e => e.Instructors)
            //    .Include(e => e.CourseSupImgs).FirstOrDefault(e => e.Id == id);

            if (courses is null)
            {
                return NotFound(new
                {
                    Message = "Course not found"
                });
            }

            var recomendCourses =(await _courserepository.GetAllAsync(expression: e => e.Name.Contains(courses.Name) && e.Categorysid == courses.Categorysid && e.Id != courses.Id,
                includes: [e => e.Categorys, e => e.Instructors],
                tracked: false,
                cancellationToken: cancellationToken
                )).Skip(0).Take(8); ;
            //_context.Courses.Where(e => e.Name.Contains(courses.Name) && e.Categorysid == courses.Categorysid && e.Id != courses.Id).Skip(0).Take(8)
            //    .Include(e => e.Categorys)
            //    .Include(e => e.Instructors)
            //    .Include(e => e.CourseSupImgs)
            //    .Skip(0)
            //    .Take(8);

            return Ok(new
            {
                course = courses,
                recomendcourses = recomendCourses.ToList()
            });
        }

        [HttpGet("TopRated/{id}")]
        public async Task<IActionResult> TopRated(int id,CancellationToken cancellationToken)
        {
            var courses = await _courserepository.GetOneAsync(e => e.Id == id, tracked: false, includes: [e => e.Categorys, e => e.Instructors], cancellationToken: cancellationToken);
            //_context.Courses.Include(e => e.Categorys)
            //    .Include(e => e.Instructors)
            //    .Include(e => e.CourseSupImgs).FirstOrDefault(e => e.Id == id);

            if (courses is null)
            {
                return NotFound(new
                {
                    Message = "Course not found"
                });
            }

            var topCourses = (await _courserepository.GetAllAsync(expression: e => e.Name.Contains(courses.Name) && e.Id != courses.Id && e.Rat > 4.5,
                includes: [e => e.Categorys, e => e.Instructors],
                tracked: false,
                cancellationToken: cancellationToken
                )).Skip(0).Take(8);

            //_context.Courses.Where(e => e.Name.Contains(courses.Name) && e.Id != courses.Id && e.Rat > 4.5)
            //    .Include(e => e.Categorys)
            //    .Include(e => e.Instructors)
            //    .Include(e => e.CourseSupImgs)
            //    .Skip(0)
            //    .Take(8);
             
            return Ok(new
            {
                course = courses,
                topratedcourses = topCourses.ToList()
            });
        }
    }
}

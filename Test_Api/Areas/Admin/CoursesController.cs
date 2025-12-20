using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Dtos.Request;
using Test_Api.Dtos.Respons;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;
using Test_Api.Utility;

namespace Test_Api.Areas.Admin
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDBcontext _context ;
        private readonly IrepositoruSupImg _repositoruSupImg;
        private readonly IRepository<Course> _courseRepository;
        public CoursesController(ApplicationDBcontext context, IrepositoruSupImg repositoruSupImg, IRepository<Course> courseRepository)
        {
            _context = context;
            _repositoruSupImg = repositoruSupImg;
            _courseRepository = courseRepository;
        }

        [HttpPost("GetAll/{page}")]
        [Authorize(Roles = $"{SD.Role_Admin},${SD.Role_Employee}")]
        public async Task<IActionResult> GetAll(FilterdataRequest filterdataRequest, CancellationToken cancellationToken , int page = 1, int pagesize = 8)
        {
            var courses =await _courseRepository.GetAllAsync(includes: [e => e.Categorys,e => e.Instructors],cancellationToken:cancellationToken);
            
            if (courses is null)
            {
                return NotFound(new
                {
                    msg = "No courses found",
                });
            }
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

            return Ok(new
            {
                data = courses.AsEnumerable(),
                filter = filterdataRespons,
                pagination = paginationRespons,
            });

        }
        [HttpGet("Get/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Get(int id,CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync(e => e.Id == id, tracked: false, includes: [e => e.Categorys, e => e.Instructors], cancellationToken: cancellationToken);
               
            if (courses is null)
            {
                return NotFound(new
                {
                    msg = "No courses found",
                });
            }
            return Ok(courses);
        }
        [HttpPost("Creat")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Creat(CreateCourseRequest createCourseRequest,CancellationToken cancellationToken)
        {
            if (createCourseRequest is null)
            {
                return BadRequest();
            }
            var course = new Course()
            {
                Name = createCourseRequest.Name,
                Description = createCourseRequest.Description,
                Price = createCourseRequest.Price,
                Discount = createCourseRequest.Discount,
                Status = createCourseRequest.Status,
                Categorysid = createCourseRequest.CategoryId,
                Instructorid = createCourseRequest.InstructorId,
                QuantityLesons= createCourseRequest.QuantitylESONS,
                totalstudents= createCourseRequest.totalstudents,
                language= createCourseRequest.language,

            };
            await _courseRepository.AddAsync(course,cancellationToken);
            
            if (createCourseRequest.Image is not null && createCourseRequest.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createCourseRequest.Image.FileName);
                var filePath = Path.Combine("wwwroot", "CourseImg", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    createCourseRequest.Image.CopyTo(stream);
                }

                course.MainImg = fileName;
               
            }
           await _courseRepository.ComitSaveAsync(cancellationToken);
            if (createCourseRequest.Videos is not null && createCourseRequest.Videos.Count > 0)
            {
                foreach (var videos in createCourseRequest.Videos)
                {
                    var supFileName = Guid.NewGuid().ToString() + Path.GetExtension(videos.FileName);
                    var supFilePath = Path.Combine("wwwroot", "CourseImg", "CourseSupimg", supFileName);
                    using (var stream = System.IO.File.Create(supFilePath))
                    {
                        videos.CopyTo(stream);
                    }
                    var courseSupImg = new CourseVideos()
                    {
                        CourseId = course.Id,
                        Video = supFileName,
                    };
                    //_context.CourseSupImgs.Add(courseSupImg);
                }
              await _repositoruSupImg.ComitSaveAsync(cancellationToken);
            }
            return CreatedAtAction(nameof(Get), new { id = course.Id }, new
            {
                msg = "Course created successfully",
            });
        }
        [HttpPut("Edit/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Edit(int id, EditCourseRequest editCourseRequest,CancellationToken cancellationToken )
        {
            if (editCourseRequest is null)
            {
                return BadRequest();
            }
            var existingCourse = await _courseRepository.GetOneAsync(e => e.Id == id,includes: [e=>e.CourseVideos], cancellationToken: cancellationToken);
         
            if (existingCourse is null)
            {
                return NotFound(new
                {
                    msg = "Course not found",
                });
            }
            existingCourse.Name = editCourseRequest.Name;
            existingCourse.Description = editCourseRequest.Description;
            existingCourse.Price = editCourseRequest.Price;
            existingCourse.Discount = editCourseRequest.Discount;
            existingCourse.Status = editCourseRequest.Status;
            existingCourse.Categorysid = editCourseRequest.CategoryId;
            existingCourse.Instructorid = editCourseRequest.InstructorId;
            existingCourse.QuantityLesons= editCourseRequest.QuantitylESONS;
            existingCourse.totalstudents= editCourseRequest.totalstudents;
            existingCourse.language= editCourseRequest.language;

            if (editCourseRequest.NewImage is not null && editCourseRequest.NewImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(editCourseRequest.NewImage.FileName);
                var filePath = Path.Combine("wwwroot", "CourseImg", fileName);
                var oldFilePath = Path.Combine("wwwroot", "CourseImg", existingCourse.MainImg ?? string.Empty);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                using (var stream = System.IO.File.Create(filePath))
                {
                    editCourseRequest.NewImage.CopyTo(stream);
                }
                existingCourse.MainImg = fileName;
            }
            if (editCourseRequest.Videos is not null && editCourseRequest.Videos.Count > 0)
            {
                var existingSupImages = await _repositoruSupImg.GetAllAsync(e => e.CourseId == existingCourse.Id, tracked: false, cancellationToken: cancellationToken);
               
                foreach (var videos in existingSupImages)
                {
                    var supImgFilePath = Path.Combine("wwwroot", "CourseImg", "CourseSupimg", videos.Video ?? string.Empty);
                    if (System.IO.File.Exists(supImgFilePath))
                    {
                        System.IO.File.Delete(supImgFilePath);
                    }
                    _context.CourseVideos.Remove(videos);
                }
                foreach (var supImage in editCourseRequest.Videos)
                {
                    var supFileName = Guid.NewGuid().ToString() + Path.GetExtension(supImage.FileName);
                    var supFilePath = Path.Combine("wwwroot", "CourseImg", "CourseSupimg", supFileName);

                    using (var stream = System.IO.File.Create(supFilePath))
                    {
                        supImage.CopyTo(stream);
                    }
                    var courseSupImg = new CourseVideos()
                    {
                        CourseId = existingCourse.Id,
                        Video = supFileName,
                    };
                    _context.CourseVideos.Add(courseSupImg);
                }
            };
            _courseRepository.Update(existingCourse);
           await _courseRepository.ComitSaveAsync(cancellationToken);
            return NoContent();

        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin}")]
        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
        {
            var existingCourse =await _courseRepository.GetOneAsync(e => e.Id == id, includes: [E => E.CourseVideos],cancellationToken:cancellationToken);
          
            if (existingCourse is null)
            {
                return NotFound(new
                {
                    msg = "Course not found",
                });
            }
            var courseImgPath = Path.Combine("wwwroot", "CourseImg", existingCourse.MainImg ?? string.Empty);
            if (System.IO.File.Exists(courseImgPath))
            {
                System.IO.File.Delete(courseImgPath);
            }
            var existingSupImages =await _repositoruSupImg.GetAllAsync(e => e.CourseId == existingCourse.Id, tracked: false, cancellationToken: cancellationToken);

            foreach (var videos in existingSupImages)
            {
                var supImgFilePath = Path.Combine("wwwroot", "CourseImg", "CourseSupimg", videos.Video ?? string.Empty);
                if (System.IO.File.Exists(supImgFilePath))
                {
                    System.IO.File.Delete(supImgFilePath);
                }
            }
            _repositoruSupImg.Removerange(existingSupImages);
            _courseRepository.Delete(existingCourse);
           await _courseRepository.ComitSaveAsync(cancellationToken);
           await _repositoruSupImg.ComitSaveAsync(cancellationToken);

            return NoContent();
        }
    }
}

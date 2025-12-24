using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;

namespace Test_Api.Areas.Students
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Students")]
    [Authorize]

    public class CheckOutController : ControllerBase
    {
        public readonly IRepository<Order> _orderRepository;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly IEmailSender _emailSender;
        public readonly IRepository<Carts> _cartsRepository;
        public readonly Irepositoruorderitem _orderitemRepository;
        public readonly IRepository<Mycourse> _mycourseRepository;
        public readonly IRepository<Course> _courseRepository;
        public readonly IRepository<CourseVideos> _CourseVideosRepository;


        public CheckOutController(IRepository<Order> orderRepository, IRepository<Mycourse> mycourseRepository, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IRepository<Carts> cartsRepository, Irepositoruorderitem orderitemRepository, IRepository<CourseVideos> courseVideosRepository, IRepository<Course> courseRepository)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _emailSender = emailSender;
            _cartsRepository = cartsRepository;
            _orderitemRepository = orderitemRepository;
            _mycourseRepository = mycourseRepository;
            _CourseVideosRepository = courseVideosRepository;
            _courseRepository = courseRepository;
        }


        [HttpGet("success/{orderId}")]
        public async Task<IActionResult> success(int orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOneAsync(e=>e.Id == orderId,cancellationToken:cancellationToken);
            if (order == null) 
                return NotFound();

            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
                return NotFound();

            //send email
            await _emailSender.SendEmailAsync(user.Email!, "ok resent order",
                $"<h1> thank total price _ {order.TotalPrice}<h1>");

            order.Status = OrderStatus.Processing;
            var service = new SessionService();
            var transaltaion = service.Get(order.sessionId);
           

            //Add cart in order
            var cart =await _cartsRepository.GetAllAsync(e => e.ApplicationUserId == order.ApplicationUserId, includes: [e => e.Courses],cancellationToken:cancellationToken);

            List<Mycourse> items = cart.Select(e=>new Mycourse
            {
                courseId = e.Courseid,
                Course = e.Courses,
                Price = e.Price,
                orderId = orderId,
                ApplicationuserId = user.Id,
            }).ToList();
           await _orderitemRepository.AddrangeAsunc(items,cancellationToken);
           await _orderitemRepository.ComitSaveAsync(cancellationToken);

            //remove cart
            foreach (var item in cart)
            {
                _cartsRepository.Delete(item);
            }
           await _cartsRepository.ComitSaveAsync(cancellationToken);

            foreach(var item in items)
            {
                var course = await _courseRepository.GetAllAsync(e => e.Id == item.courseId,cancellationToken:cancellationToken);
                foreach(var courseItem in course)
                {
                    courseItem.totalstudents += 1;
                }
            }
            await _courseRepository.ComitSaveAsync(cancellationToken);

            return RedirectToAction(nameof(GetCoursesBuy));
        }

        [HttpGet("cancel/{orderId}")]
        public async Task<IActionResult> cancel(int orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == orderId,cancellationToken:cancellationToken);
            if (order == null)
                return NotFound();

            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
                return NotFound();

            await _emailSender.SendEmailAsync(user.Email!, "No order is not Completed",
                $"No order is canceled because is not Completed");

            order.Status = OrderStatus.Cancelled;
            await _orderitemRepository.ComitSaveAsync(cancellationToken);

            return BadRequest(new
            {
                msg="canceld buy course"
            });
        }

        [HttpGet("GetCoursesBuy")]
        public async Task<IActionResult> GetCoursesBuy(CancellationToken cancellationToken)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound();

            var courses = await _mycourseRepository.GetAllAsync(e => e.ApplicationuserId == user.Id,
                includes: [e => e.Course.CourseVideos], cancellationToken: cancellationToken);

            return Ok(courses.Select(e => new
            {
              e.Course,
              e.Order,
            }));
        }
    }
}

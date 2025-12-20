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
    [Route("api/[controller]")]
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

        public CheckOutController(IRepository<Order> orderRepository, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IRepository<Carts> cartsRepository, Irepositoruorderitem orderitemRepository)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _emailSender = emailSender;
            _cartsRepository = cartsRepository;
            _orderitemRepository = orderitemRepository;
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> Successd(int orderId , CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOneAsync(e=>e.Id == orderId);
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
            }).ToList();
           await _orderitemRepository.AddrangeAsunc(items,cancellationToken);
           await _orderitemRepository.ComitSaveAsync(cancellationToken);

            //remove cart
            foreach (var item in cart)
            {
                _cartsRepository.Delete(item);
            }
           await _cartsRepository.ComitSaveAsync(cancellationToken);

            return Ok();
        }

        public IActionResult Canceld()
        {
            return Ok();
        }
    }
}

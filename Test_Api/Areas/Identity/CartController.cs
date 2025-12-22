using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;
using Test_Api.Utility;

namespace Test_Api.Areas.Identity
{
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    [Authorize]

    public class CartController : ControllerBase
    {
        public readonly IRepository<Course> _courserepository;
        public readonly IRepository<Carts> _cartrepository;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly IRepository<Promotion> _promotionrepository;
        public readonly IRepository<Order> _orderrepository;

        public CartController(IRepository<Course> courserepository, IRepository<Carts> repositorycart, UserManager<ApplicationUser> userManager, IRepository<Promotion> promotionrepository, IRepository<Order> orderrepository)
        {
            _courserepository = courserepository;
            _cartrepository = repositorycart;
            _userManager = userManager;
            _promotionrepository = promotionrepository;
            _orderrepository = orderrepository;
        }

        [HttpPut("GetCartItems/{code?}")]
        public async Task<IActionResult> GetCartItems(string? code,CancellationToken cancellationToken)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userid!);
            if (user is null)
            {
                return NotFound(new
                {
                    msg = "No user found",
                });
            }
            var cartitems = await _cartrepository.GetAllAsync(e => e.ApplicationUserId == user.Id && e.Courses.Status , includes: [e=>e.ApplicationUsers,e=>e.Courses],cancellationToken:cancellationToken);

            if (code is not null)
            {
                var promotion = await _promotionrepository.GetOneAsync(e => e.Code == code && e.IsActive, cancellationToken: cancellationToken);
                if (promotion is not null)
                {
                    var result = cartitems.FirstOrDefault(e => e.Courseid == promotion.CourseId);
                    if (result is not null)
                    {
                        result.Price = result.Price - (decimal)(result.Price * promotion.DiscountPercentage / 100);
                    }
                    await _cartrepository.ComitSaveAsync(cancellationToken);
                }
            }
            return Ok(cartitems);
        }
        [Authorize]
        [HttpPost("Addtocart/{coursid}")]
        public async Task<IActionResult> Addtocart(int coursid,CancellationToken cancellationToken)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userid !);
            if (user is null)
            {
                return NotFound(new
                {
                    msg = "No user found",
                });
            }
            var coursincart = await _cartrepository.GetOneAsync( e => e.Courseid == coursid && e.ApplicationUserId == user.Id,cancellationToken:cancellationToken);

            if (coursincart is not null)
            {
                return BadRequest(new
                {
                    msg = "Course already in cart",
                });
            }
           var course = await _courserepository.GetOneAsync(e => e.Id == coursid && e.Status, tracked: false, cancellationToken: cancellationToken);
            var cart = new Carts()
            {
                ApplicationUserId = user.Id,
                Courseid = coursid,
                Price = course.Price - (decimal) (course.Price  * course.Discount / 100)
            };
             await _cartrepository.AddAsync(cart,cancellationToken);
            await _cartrepository.ComitSaveAsync(cancellationToken);
            return  Created();
        }
        [HttpDelete("Delete/{coureid}")]
        public async Task<IActionResult> Delete(int coureid,CancellationToken cancellationToken)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userid!);
            if (user is null)
            {
                return NotFound(new
                {
                    msg = "No user found",
                });
            }

            var cartitem = await _cartrepository.GetOneAsync(e => e.Courseid == coureid && e.ApplicationUserId == user.Id,cancellationToken:cancellationToken);

            if (cartitem is null)
            {
                return NotFound(new
                {
                    msg = "No cart item found",
                });
            }

             _cartrepository.Delete(cartitem);
            await _cartrepository.ComitSaveAsync(cancellationToken);
            return NoContent();
        }
        [HttpPost("Pay")]
        public async Task<IActionResult> Pay(CancellationToken cancellationToken)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid is null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartrepository.GetAllAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Courses]);
            if (cart is null) return NotFound();

            var order = new Order
            {
                TotalPrice = cart.Sum(e => e.Price),
                ApplicationUserId = user.Id,
            };
           await _orderrepository.AddAsync(order,cancellationToken);

            //var order = await _orderrepository.AddAsync(new()
            //{
            //    TotalPrice= cart.Sum(e=>e.Price),
            //    ApplicationUserId=user.Id,
            //},cancellationToken);

            await _orderrepository.ComitSaveAsync(cancellationToken);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Students/CheckOut/success/{order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Students/CheckOut/cancel/{order.Id}",
            };

            foreach (var item in cart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Courses.Name,
                            Description = item.Courses.Description,
                        },
                        UnitAmount = (long)item.Price * 100,
                    },
                    Quantity = 1,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            order.sessionId = session.Id;

           await _orderrepository.ComitSaveAsync(cancellationToken);

            return Ok(session.Url);
        }


    }
}

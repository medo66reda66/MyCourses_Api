using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Test_Api.Models;
using Test_Api.Utility;

namespace Test_Api.Areas.Admin
{
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin}")]
    public class UsersController : ControllerBase
    {
        public readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Getuser()
        {
            var users = _userManager.Users.Select(e =>
            new
            {
                e.Email,
                e.FirstName,
                e.LastName,
                e.PhoneNumber,
                e.LockoutEnabled
            }).ToList();

            return Ok(users);
        }

        [HttpPost("LockUnLock/{usid}")]
        public async Task<IActionResult> LockUnLock(string usid)
        {
            var user = await _userManager.FindByIdAsync(usid);
            if (user == null)
                return NotFound();

            if(await _userManager.IsInRoleAsync(user,SD.Role_Admin))
                return BadRequest("Cannot lock/unlock a Super Admin user.");

            user.LockoutEnabled = !user.LockoutEnabled;

            if (!user.LockoutEnabled)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(30);
            }
            else
            {
                user.LockoutEnd = null;
            }

           await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}

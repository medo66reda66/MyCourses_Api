using Ecommers.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test_Api.Dtos.Respons;
using Test_Api.Models;

namespace Test_Api.Areas.Identity
{
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    [Authorize]
    public class UpdateProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UpdateProfileController( UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet("GetInfo")]
        public IActionResult GetInfo(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user is null)
            {
                return NotFound(new
                {
                    msg = "No user found",
                });
            }
            var userinfo = new ApplicationuserRespons()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = $"{user.FirstName}{user.LastName}",
                Address = user.Address ?? string.Empty,
            };

            return Ok(userinfo);
        }
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(ApplicationuserRespons applicationuserRespons)
        {
            var user =  _userManager.FindByIdAsync(applicationuserRespons.Id).Result;
            if (user is null)
            {
                return NotFound(new
                {
                    msg = "No user found",
                });
            }
            var name = applicationuserRespons.FullName.Split(' ');

            user.Email = applicationuserRespons.Email;
            user.PhoneNumber = applicationuserRespons.PhoneNumber;
            user.Address = applicationuserRespons.Address;
            if (name.Length > 1)
            {
                user.FirstName = name[0];
                user.LastName = name[1];
            }
             await _userManager.UpdateAsync(user);

            return Ok(new
            {
                msg = "Update successful",
            });
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ApplicationuserRespons applicationuserRespons)
        {
            var user =  _userManager.FindByIdAsync(applicationuserRespons.Id).Result;
            if (user is null)
            {
                return NotFound(new
                {
                    msg = "No user found",
                });
            }

            if (applicationuserRespons.CurrentPassword is null || applicationuserRespons.NewPassword is null)
                return BadRequest("Current password and new password are required.");

            var result = await _userManager.ChangePasswordAsync(user, applicationuserRespons.CurrentPassword!, applicationuserRespons.NewPassword!);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    msg = "Password change failed",
                });
            }

            return Ok(new
            {
                msg = "Password changed successfully",
            });
        }

    }
}

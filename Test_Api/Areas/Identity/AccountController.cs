using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Test_Api.Areas.Students;
using Test_Api.Dtos.Request;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;
using Test_Api.Servers;
using Test_Api.Utility;
using static System.Net.WebRequestMethods;

namespace Test_Api.Areas.Identity
{
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountController : ControllerBase
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IToken _tokenservice;
        public readonly IEmailSender _emailSender;
        public readonly IRepository<ApplicationuserOTP> _applicationuserOTPrepository;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IToken tokenservice, IEmailSender emailSender, IRepository<ApplicationuserOTP> applicationuserOTPrepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenservice = tokenservice;
            _emailSender = emailSender;
            _applicationuserOTPrepository = applicationuserOTPrepository;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var user = new ApplicationUser()
            {
                UserName = registerRequest.EmailorUsername,
                Email = registerRequest.EmailorUsername,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                PhoneNumber = registerRequest.PhoneNumber,
                Address = registerRequest.Address,
                City = registerRequest.City,
            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    msg = "User registration failed",
                    errors = result.Errors.Select(e => e.Description)
                });
            }
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var Link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token=token }, Request.Scheme, Request.Host.ToString());

            // await _emailSender.SendEmailAsync(
            //    user.Email,
            //    "Confirm your email",
            //    $"Please confirm your account by <a href='{Link}'>clicking here</a>.");

            await _signInManager.SignInAsync(user, isPersistent: registerRequest.Memberme);
            await _userManager.AddToRoleAsync(user, SD.Role_User);

            return Ok(new
            {
                msg = "User registered and logged in successfully"
            });
        }

        //[HttpGet("ConfirmEmail")]
        //public async Task<IActionResult> ConfirmEmail(string userId, string token)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return NotFound(new
        //        {
        //            msg = "User not found"
        //        });
        //    }
        //    var result = await _userManager.ConfirmEmailAsync(user, token);
        //    if (result.Succeeded)
        //    {
        //        return Ok(new
        //        {
        //            msg = "Email confirmed successfully"
        //        });
        //    }
        //    else
        //    {
        //        return BadRequest(new
        //        {
        //            msg = "Email confirmation failed",
        //            errors = result.Errors.Select(e => e.Description)
        //        });
        //    }
        //}

        [HttpPost("Forgetpassword")]
        public async Task<IActionResult> Forgetpassword(ForgetpasswordRequest forgetpasswordRequest,CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(forgetpasswordRequest.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    msg = "User not found"
                });
            }
           
            var totalvalidOtps = await _applicationuserOTPrepository.GetAllAsync(
                e => e.ApplicationUserId == user.Id ,
                cancellationToken: cancellationToken);

            var totalOTPs = totalvalidOtps.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totalOTPs > 5)
                return BadRequest(new
                {
                    Message = "You have exceeded the maximum number of OTP requests. Please try again later."
                });

            var otp = new Random().Next(100000, 999999).ToString();

            await _applicationuserOTPrepository.AddAsync(new ApplicationuserOTP
            {
               ApplicationUserId = user.Id,
               OTP = otp,
               isvalid = true,
               CreateAt = DateTime.UtcNow,
               Validto = DateTime.UtcNow.AddDays(1)
            },cancellationToken);
            await _applicationuserOTPrepository.ComitSaveAsync(cancellationToken);

           await _emailSender.SendEmailAsync(
               user.Email,
               "Your OTP Code",
               $"Your OTP code is: {otp}. It is valid for 24 hours.");

            return Ok(new
            {
                msg = "OTP sent to email successfully",
                OTP = otp
            });
        }
        [HttpPost("validateOTP")]
        public async Task<IActionResult> validateOTP(ValidateOtpRequest validateOTPRequest,CancellationToken cancellationToken)
        {
           
            var otpRecord = await _applicationuserOTPrepository.GetOneAsync(
                e => e.ApplicationUserId == validateOTPRequest.ApplicationUserId && e.OTP == validateOTPRequest.OTP && e.isvalid,
                cancellationToken: cancellationToken);

            if (otpRecord is null)
                return CreatedAtAction(nameof(validateOTP), new { userId = validateOTPRequest.ApplicationUserId }, new
                {
                    Message = "Invalid OTP"
                });

            return CreatedAtAction(nameof(NewPassword), new { userId = validateOTPRequest.ApplicationUserId }, new
            {
                Message = "OTP validated successfully. You can now reset your password."
            });
        }
        [HttpGet("ResentOtp")]
        public async Task<IActionResult> ResentOtp(string ApplicationUserId , CancellationToken cancellationToken)
        {
            var userid = await _applicationuserOTPrepository.GetOneAsync( e => e.ApplicationUserId == ApplicationUserId,includes: [e => e.ApplicationUser],cancellationToken:cancellationToken);
            if (userid == null)
            {
                return NotFound(new
                {
                    msg = "User not found"
                });
            }
            userid.isvalid = false;
            
            var totalvalidOtps = await _applicationuserOTPrepository.GetAllAsync(
               e => e.ApplicationUserId == userid.ApplicationUserId ,
               cancellationToken: cancellationToken);

            var totalOTPs = totalvalidOtps.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totalOTPs >= 5)
                return BadRequest(new
                {
                    Message = "You have exceeded the maximum number of OTP requests. Please try again later."
                });

             var otp = new Random().Next(100000, 999999).ToString();

            await _applicationuserOTPrepository.AddAsync(new ApplicationuserOTP
            {
                ApplicationUserId = userid.ApplicationUserId,
                OTP = otp,
                isvalid = true,
                CreateAt = DateTime.UtcNow,
                Validto = DateTime.UtcNow.AddHours(8)
            }, cancellationToken);
            await _applicationuserOTPrepository.ComitSaveAsync(cancellationToken);
           await _emailSender.SendEmailAsync(
               userid.ApplicationUser.Email,
               "Your OTP Code",
               $"Your OTP code is resent: {otp}. It is valid for 8 hours.");


            return Ok(new
            {
                msg = "Resend OTP endpoint"
            });
        }
        [HttpPost("NewPassword")]
        public async Task<IActionResult> NewPassword(NewPasswordRequest newPasswordRequest,CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(newPasswordRequest.ApplicationUserId);
            if (user == null)
            {
                return NotFound(new
                {
                    msg = "User not found"
                });
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    msg = "Password reset failed",
                    errors = result.Errors.Select(e => e.Description)
                });
            }
            var otpRecord = await _applicationuserOTPrepository.GetOneAsync(
                e => e.ApplicationUserId == newPasswordRequest.ApplicationUserId && e.OTP == newPasswordRequest.otp && e.isvalid,
                cancellationToken: cancellationToken);
            if (otpRecord != null)
            {
                otpRecord.isvalid = false;
            }
            return RedirectToAction(nameof(Login));

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.EmailorUsername);
            if (user == null)
            {
                return NotFound(new
                {
                    msg = "User not found"
                });
            }
            user.EmailConfirmed = true;
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.Memberme, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return BadRequest(new
                    {
                        msg = "Account is locked due to multiple failed login attempts. Please try again later"
                    });
                }
                else 
                {
                    return BadRequest(new
                    {
                        msg = "Invalid login attempt"
                    });
                }
              
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
             {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Role, string.Join(",",userRoles)),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var accessToken = _tokenservice.GenerateAccessToken(claims);
            var refreshToken = _tokenservice.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = accessToken,  
                expire="30 minutes",
                RefreshToken = refreshToken,
                msg = "User logged in successfully"
            });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}

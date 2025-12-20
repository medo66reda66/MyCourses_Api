using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class ValidateOtpRequest
    {
        [Required]
        public string OTP { get; set; } = string.Empty;

        public string ApplicationUserId { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class NewPasswordRequest
    {
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } 
        [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string confirmPassword { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
    }
}

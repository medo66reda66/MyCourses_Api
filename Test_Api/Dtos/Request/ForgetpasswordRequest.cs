using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class ForgetpasswordRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}

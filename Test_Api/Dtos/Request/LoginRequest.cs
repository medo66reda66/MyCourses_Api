using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string EmailorUsername { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public bool Memberme { get; set; }
    }
}

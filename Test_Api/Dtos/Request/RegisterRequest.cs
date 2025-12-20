using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Request
{
    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required,EmailAddress]
        public string EmailorUsername { get; set; } = string.Empty;
        [Required,DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password),Compare(nameof(Password))]
        public string confirmPassword { get; set; } = string.Empty; 
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public bool Memberme { get; set; } = false;
    }
}

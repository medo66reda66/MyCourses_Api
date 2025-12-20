using System.ComponentModel.DataAnnotations;

namespace Test_Api.Dtos.Respons
{
    public class ApplicationuserRespons
    {
        public string Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}

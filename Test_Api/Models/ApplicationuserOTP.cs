namespace Test_Api.Models
{
    public class ApplicationuserOTP
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } 
        public ApplicationUser? ApplicationUser { get; set; }
        public string OTP { get; set; } 
        public bool isvalid { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime Validto { get; set; }
    }
}

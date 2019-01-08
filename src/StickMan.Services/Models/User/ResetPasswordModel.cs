namespace StickMan.Services.Models.User
{
    public class ResetPasswordModel
    {
        public string SessionToken { get; set; }
        public string NewPassword { get; set; }
    }
}
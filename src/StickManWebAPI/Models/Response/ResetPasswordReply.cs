using System.Net;

namespace StickManWebAPI.Models.Response
{
    public class ResetPasswordReply : Reply
    {
        public ResetPasswordReply() : base(HttpStatusCode.OK, "Authenticated user")
        {
        }

        public StickMan.Services.Models.User.ResetPasswordModel User { get; set; }
    }
}
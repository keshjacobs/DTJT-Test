using StickManWebAPI.Models.Request;

namespace StickManWebAPI.Models
{
    public class ReplyPostModel : CastMessageToUpload
    {
        public int CastMessageId { get; set; }

        public string Comment { get; set; }
    }
}
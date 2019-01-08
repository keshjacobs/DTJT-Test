using StickManWebAPI.Models.Request;

namespace StickManWebAPI.Models
{
    public class RepostModel : SessionData
    {
        public int CastMessageId { get; set; }
        public string Comment { get; set; }
    }
}
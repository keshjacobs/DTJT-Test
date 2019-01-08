namespace StickManWebAPI.Models.Response
{
    public class LikePostReply : Reply
    {
        public LikePostReply()
        {
            replyCode = (int)EnumReply.processOk;
            replyMessage = "Like post message clicked";
        }

        public int LikePostCount { get; set; }
    }
}
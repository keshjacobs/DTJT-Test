namespace StickManWebAPI.Models.Response
{
    public class DislikePostReply : Reply
    {
        public DislikePostReply()
        {
            replyCode = (int)EnumReply.processOk;
            replyMessage = "Like post message clicked";
        }

        public int DislikePostCount { get; set; }
    }
}
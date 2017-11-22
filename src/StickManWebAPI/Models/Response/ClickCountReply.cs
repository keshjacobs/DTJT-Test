namespace StickManWebAPI.Models.Response
{
	public class ClickCountReply : Reply
	{
		public ClickCountReply()
		{
			replyCode = (int)EnumReply.processOk;
			replyMessage = "Cast message clicked";
		}

		public int ClickCount { get; set; }
	}
}
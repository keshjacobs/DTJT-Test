namespace StickManWebAPI.Models.Response
{
	public class ChangeTitleReply : Reply
	{
		public ChangeTitleReply()
		{
			replyCode = (int) EnumReply.processOk;
			replyMessage = "Cast title changed";
		}

		public string Title { get; set; }
	}
}
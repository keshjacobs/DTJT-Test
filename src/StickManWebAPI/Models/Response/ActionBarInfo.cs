namespace StickManWebAPI.Models.Response
{
	public class ActionBarInfo : Reply
	{
		public ActionBarInfo()
		{
			replyCode = 200;
		}

		public int UnreadMessages { get; set; }
        public int UnreadNotification { get; set; }

        public int UnansweredFriendRequests { get; set; }
	}
}
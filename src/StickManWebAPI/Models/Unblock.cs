namespace StickManWebAPI.Models
{
	public class Unblock
	{
		public int ReceiverId { get; set; }

		public int UserId { get; set; }

		public string SessionToken { get; set; }
	}
}
namespace StickManWebAPI.Models
{
	public class FileContent
	{
		public int UserId { get; set; }

		public string SessionToken { get; set; }

		public string FileName { get; set; }
	}

	public class Base64FileContent : FileContent
	{
		public string Base64Content { get; set; }
	}
}
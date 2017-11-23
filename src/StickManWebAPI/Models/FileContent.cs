using System;

namespace StickManWebAPI.Models
{
	[Obsolete]
	public class FileContent
	{
		public int UserId { get; set; }

		public string SessionToken { get; set; }

		public string FileName { get; set; }
	}

	[Obsolete]
	public class Base64FileContent : FileContent
	{
		public string Base64Content { get; set; }
	}
}
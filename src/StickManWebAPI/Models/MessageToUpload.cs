using StickManWebAPI.Models.Request;

namespace StickManWebAPI.Models
{
	public class MessageToUpload : SessionData
	{
		public string FileName { get; set; }

		public string Base64Content { get; set; }
	}
}
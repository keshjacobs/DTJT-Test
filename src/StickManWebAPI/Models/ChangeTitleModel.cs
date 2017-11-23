using StickManWebAPI.Models.Request;

namespace StickManWebAPI.Models
{
	public class ChangeTitleModel : SessionData
	{
		public int CastMessageId { get; set; }

		public string NewTitle { get; set; }
	}
}
using System;

namespace StickManWebAPI.Models.Request
{
	public class CastMessageToUpload : MessageToUpload
	{
		public string Title { get; set; }
	}

	[Obsolete]
	public class ObsoleteCastMessageToUpload : SessionData
	{
		public string FileName { get; set; }

		public string FilePath { get; set; }

		public string Title { get; set; }
	}
}
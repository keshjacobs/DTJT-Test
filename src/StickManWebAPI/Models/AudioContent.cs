using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StickManWebAPI.Models
{
	public class AudioWrapper
	{
		public Reply reply { get; set; }
		public PushInfo pushInfo { get; set; }
	}

	public class AudioContent
	{
		public string userId { get; set; }
		public List<string> recieverId { get; set; }
		public string filePath { get; set; }
		public string sessionToken { get; set; }
		public string filter { get; set; }
	}
}
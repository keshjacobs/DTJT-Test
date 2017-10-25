using System.Collections.Generic;

namespace StickManWebAPI.Models
{
	public class AudioMessagesWrapper
	{
		public Reply reply { get; set; }

		public List<AudioMessage> audioMessages { get; set; }

		public List<NewAudioMessage> newaudioMessages { get; set; }
	}
}
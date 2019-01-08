using System;
using StickMan.Services.Models.User;

namespace StickMan.Services.Models.Message
{
	public class CastMessage
	{
		public CastAudioInfo MessageInfo { get; set; }

		public UserModel User { get; set; }

		public bool Listened { get; set; }
	}

	public class CastAudioInfo : AudioInfo
	{
		public string Title { get; set; }

		public string TimePassedSinceUploaded { get; set; }

		public string Duration { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public int Replied { get; set; }

        public int Reposted { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public CastMessage OriginalMessageInfo { get; set; }
    }

	public class AudioInfo
	{
		public int Id { get; set; }

		public string AudioFilePath { get; set; }

		public DateTime UploadTime { get; set; }

		public int Clicks { get; set; }
	}
}

using System;
using StickMan.Services.Contracts;
using TagLib;

namespace StickMan.Services.Implementation
{
	public class AudioFileService : IAudioFileService
	{
		private readonly IPathProvider _pathProvider;

		public AudioFileService(IPathProvider pathProvider)
		{
			_pathProvider = pathProvider;
		}

		public TimeSpan GetDuration(string relativePath)
		{
			var fullPath = _pathProvider.BuildAudioPath(relativePath);
			try
			{
				var audioFile = File.Create(fullPath, ReadStyle.Average);
				return audioFile.Properties.Duration;
			}
			catch (Exception ex)
			{
				return TimeSpan.Zero;
			}
		}
	}
}
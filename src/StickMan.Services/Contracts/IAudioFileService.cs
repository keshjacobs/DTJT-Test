using System;

namespace StickMan.Services.Contracts
{
	public interface IAudioFileService
	{
		TimeSpan GetDuration(string relativePath);
	}
}

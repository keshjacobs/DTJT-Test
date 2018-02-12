using System;

namespace StickMan.Services.Extensions
{
	public static class TimeSpanExtensions
	{
		public static string FormatDuration(this TimeSpan duration)
		{
			return duration.ToString(@"dd\:hh\:mm\:ss");
		}
	}
}

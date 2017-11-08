using FluentScheduler;
using StickManWebAPI.Scheduler.Jobs;

namespace StickManWebAPI.Scheduler
{
	public class JobsRegistry : Registry
	{
		public JobsRegistry()
		{
			Schedule<RemoveMessagesJob>().ToRunNow().AndEvery(2).Days();
		}
	}
}
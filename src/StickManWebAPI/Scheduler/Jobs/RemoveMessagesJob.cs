using System.Web.Hosting;
using FluentScheduler;
using StickMan.Services.Contracts;

namespace StickManWebAPI.Scheduler.Jobs
{
	public class RemoveMessagesJob : IJob, IRegisteredObject
	{
		private readonly IMessageService _messageService;
		private readonly object _lock = new object();

		private bool _shuttingDown;

		public RemoveMessagesJob(IMessageService messageService)
		{
			_messageService = messageService;
			// Register this job with the hosting environment.
			// Allows for a more graceful stop of the job, in the case of IIS shutting down.
			HostingEnvironment.RegisterObject(this);
		}

		public void Execute()
		{
			lock (_lock)
			{
				if (_shuttingDown)
				{
					return;
				}

				_messageService.CleanUpMessages();
			}
		}

		public void Stop(bool immediate)
		{
			// Locking here will wait for the lock in Execute to be released until this code can continue.
			lock (_lock)
			{
				_shuttingDown = true;
			}

			HostingEnvironment.UnregisterObject(this);
		}
	}
}
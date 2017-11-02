using FluentScheduler;
using Microsoft.Practices.Unity;

namespace StickManWebAPI.Scheduler
{
	public class StructureMapJobFactory : IJobFactory
	{
		private readonly UnityContainer _container;

		public StructureMapJobFactory(UnityContainer container)
		{
			_container = container;
		}

		public IJob GetJobInstance<T>() where T : IJob
		{
			return _container.Resolve<T>();
		}
	}
}
using StickMan.Database.Repository;

namespace StickMan.Database.UnitOfWork
{
	public interface IUnitOfWork
	{
		void Save();

		IRepository<TData> Repository<TData>() where TData : class;
	}
}

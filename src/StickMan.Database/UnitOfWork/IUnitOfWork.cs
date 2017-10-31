using StickMan.Database.Repository;

namespace StickMan.Database.UnitOfWork
{
	public interface IUnitOfWork
	{
		void Save();

		IFriendRequestRepository FriendRequestRepository { get; }
	}
}

using StickMan.Database.Repository.Contracts;

namespace StickMan.Database.UnitOfWork
{
	public interface IUnitOfWork
	{
		void Save();

		IFriendRequestRepository FriendRequestRepository { get; }

		IMessageRepository MessageRepository { get; }

		IUserRepository UserRepository { get; }
	}
}

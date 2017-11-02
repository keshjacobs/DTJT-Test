using StickMan.Database.Repository;
using StickMan.Database.Repository.Contracts;

namespace StickMan.Database.UnitOfWork
{
	public interface IUnitOfWork
	{
		void Save();

		IFriendRequestRepository FriendRequestRepository { get; }

		IAudioDataUploadInfoRepository AudioDataUploadInfoRepository { get; }

		IUserRepository UserRepository { get; }
	}
}

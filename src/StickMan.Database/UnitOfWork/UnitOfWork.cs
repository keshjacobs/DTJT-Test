using System;
using StickMan.Database.Repository;

namespace StickMan.Database.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private IFriendRequestRepository _friendRequestRepository;

		private readonly EfStickManContext _context;
		private bool _disposed;

		public UnitOfWork()
		{
			_context = new EfStickManContext();
		}

		public void Save()
		{
			_context.SaveChanges();
		}

		public IFriendRequestRepository FriendRequestRepository => _friendRequestRepository ?? (_friendRequestRepository = new FriendRequestRepository(_context));

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_context.Dispose();
				}
			}
			_disposed = true;
		}
	}
}
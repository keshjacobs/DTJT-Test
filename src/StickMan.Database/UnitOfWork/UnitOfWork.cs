using System;
using System.Collections;
using StickMan.Database.Repository;

namespace StickMan.Database.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly EfStickManConnectionString _context;
		private bool _disposed;
		private readonly Hashtable _repositories;

		public UnitOfWork()
		{
			_repositories = new Hashtable();
			_context = new EfStickManConnectionString();
		}

		public void Save()
		{
			_context.SaveChanges();
		}

		public IRepository<TData> Repository<TData>() where TData : class
		{
			var type = typeof(TData).Name;

			if (!_repositories.ContainsKey(type))
			{
				_repositories.Add(type, new Repository<TData>(_context));
			}

			return (IRepository<TData>)_repositories[type];
		}

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

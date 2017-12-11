using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace StickMan.Database.Repository
{
	public class Repository<TEntity> : IRepository<TEntity>
		where TEntity : class
	{
		private readonly EfStickManContext _context;
		private readonly IDbSet<TEntity> _entities;

		public Repository(EfStickManContext context)
		{
			_context = context;
			_entities = context.Set<TEntity>();
		}

		public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter)
		{
			return GetQuery(filter).ToList();
		}

		public IEnumerable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter)
		{
			return _entities.Where(filter);
		}

		public TEntity GetSingle(Expression<Func<TEntity, bool>> filter)
		{
			return Get(filter).Single();
		}

		public IEnumerable<TEntity> GetAll()
		{
			return GetQueryAll().ToList();
		}

		public IQueryable<TEntity> GetQueryAll()
		{
			return _entities;
		}

		public void Insert(TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			_entities.Add(entity);
		}

		public void Update(TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			try
			{
				_context.Set<TEntity>().Attach(entity);
			}
			catch (Exception e)
			{
				_context.Entry(entity).State = EntityState.Detached;
				_context.Set<TEntity>().Attach(entity);
			}

			_context.Entry(entity).State = EntityState.Modified;
		}

		public void Delete(TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			_entities.Remove(entity);
		}
	}
}

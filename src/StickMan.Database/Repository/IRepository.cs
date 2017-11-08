using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StickMan.Database.Repository
{
	public interface IRepository<TEntity>
	{
		IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter);

		TEntity GetSingle(Expression<Func<TEntity, bool>> filter);

		IEnumerable<TEntity> GetAll();

		IQueryable<TEntity> GetQueryAll();

		void Insert(TEntity entity);

		void Update(TEntity entity);

		void Delete(TEntity entity);
	}
}

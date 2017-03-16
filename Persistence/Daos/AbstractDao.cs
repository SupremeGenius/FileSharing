using System;
using DocumentManager.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManager.Persistence.Daos
{
	public abstract class AbstractDao<T, PK> : IDisposable where T : AbstractModel
	{
		protected DocumentManagerContext _context;
		protected DbSet<T> _dbSet;

		public AbstractDao()
		{
			_context = new DocumentManagerContext();
			_dbSet = _context.Set<T>();
		}

		#region IDisposable

		public void Dispose()
		{
		}

		#endregion

		#region CRUD 

		public T Create(T entity)
		{
			_dbSet.Add(entity);
			_context.SaveChanges();
			return entity;
		}

		public T Read(PK idEntity)
		{
			return _dbSet.Find(idEntity);
		}

		public void Update(T entity)
		{
			_context.Entry(entity).State = EntityState.Modified;
			_context.SaveChanges();
		}

		public void Delete(T entity)
		{
			_dbSet.Remove(entity);
			_context.SaveChanges();
		}

		#endregion
	}
}

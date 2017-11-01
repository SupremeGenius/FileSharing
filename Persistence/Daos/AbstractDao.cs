using System;
using FileSharing.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using FileSharing.Persistence.Context;

namespace FileSharing.Persistence.Daos
{
	public abstract class AbstractDao<T, PK> : IDisposable where T : AbstractModel
	{
		protected DatabaseContext _context;
		protected DbSet<T> _dbSet;

		public AbstractDao()
        {
            using (var factory = new ContextFactory())
            {
                _context = factory.CreateDbContext(new string[0]);
            }
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
            _context.Entry(entity).State = EntityState.Deleted;
            _dbSet.Remove(entity);
			_context.SaveChanges();
		}

		#endregion
	}
}

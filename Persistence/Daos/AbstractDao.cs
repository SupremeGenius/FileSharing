using System;
using DocumentManager.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DocumentManager.Persistence.Daos
{
	public abstract class AbstractDao<T, PK> : IDisposable where T : AbstractModel
	{
		protected DocumentManagerContext _context;
		protected DbSet<T> _dbSet;

		public AbstractDao()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("db_settings.json");
            var configuration = configurationBuilder.Build();

            DbContextOptionsBuilder<DocumentManagerContext> optionsBuilder = new DbContextOptionsBuilder<DocumentManagerContext>();
            optionsBuilder.UseSqlServer($"{configuration["ConnectionString"]}");

            _context = new DocumentManagerContext(optionsBuilder.Options);
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

using System;
using FileSharing.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FileSharing.Persistence.Daos
{
	public abstract class AbstractDao<T, PK> : IDisposable where T : AbstractModel
	{
		protected FileSharingContext _context;
		protected DbSet<T> _dbSet;

		public AbstractDao()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("db_settings.json");
            var configuration = configurationBuilder.Build();

            DbContextOptionsBuilder<FileSharingContext> optionsBuilder = new DbContextOptionsBuilder<FileSharingContext>();
            optionsBuilder.UseSqlServer($"{configuration["ConnectionString"]}");

            _context = new FileSharingContext(optionsBuilder.Options);
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

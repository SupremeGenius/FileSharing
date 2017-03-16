using System;
using DocumentManager.Services.Mapping;

namespace DocumentManager.Services
{
	public abstract class AbstractServices<T> : IDisposable
	{
		protected T _dao;

		public AbstractServices(T dao)
		{
			AutoMapperConfig.RegisterMappings();
			_dao = dao;
		}

		#region IDisposable

		public void Dispose()
		{
		}

		#endregion
	}
}

using System;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Mapping;

namespace DocumentManager.Services
{
	public abstract class AbstractServices<T> : IDisposable
	{
		protected T _dao;
		protected SessionServices _sessionServices;
		AuditServices _auditServices;

		public AbstractServices(T dao)
		{
			AutoMapperConfig.RegisterMappings();
			_dao = dao;
			_sessionServices = new SessionServices();
			_auditServices = new AuditServices();
		}

		#region IDisposable

		public void Dispose()
		{
		}

		#endregion

		public SessionDto CheckSession(string securityToken)
		{
			return _sessionServices.Read(securityToken);
		}

		public void Audit(string idObj, string obj, string action, long idUser)
		{
			try
			{
				var audit = new AuditDto
				{
					Action = action,
					IdObject = idObj,
					IdUser = idUser,
					Object = obj
				};
				using (var auditService = new AuditServices())
				{
					auditService.Create(audit);
				}
			}
			catch (Exception)
			{
				//TODO Implement a log.
			}
		}
	}
}

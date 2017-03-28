using System;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;
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

		public SessionDto CheckSession(string securityToken)
		{
            using (var _sessionServices = new SessionServices())
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
				using (var _auditService = new AuditServices())
					_auditService.Create(audit);
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}

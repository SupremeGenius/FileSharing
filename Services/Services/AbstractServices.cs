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

		public void Audit(long idUser, string idObj, string obj, ActionDto action, string description)
		{
			try
			{
				var audit = new AuditDto
                {
                    IdUser = idUser,
					IdObject = idObj,
					Object = obj,
                    Action = action,
                    Description = description
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

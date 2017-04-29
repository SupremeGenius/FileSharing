using System;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using FileSharing.Services.Mapping;

namespace FileSharing.Services
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
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}

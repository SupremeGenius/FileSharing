using System;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;

namespace DocumentManager.Services
{
	public class SessionServices : AbstractServices<SessionDao>
	{
		public SessionServices() : base(new SessionDao()) { }

		public string Create(long idUser)
		{
			try
			{
				var session = new Session
				{
					SecurityToken = Guid.NewGuid().ToString(),
					IdUser = idUser,
					DateLastAccess = DateTime.Now
				};
				_dao.Create(session);

				return session.SecurityToken;
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public SessionDto Read(string securityToken)
		{
			try
			{
				var session = _dao.Read(securityToken);
				if (session == null)
					throw new DocumentManagerException(DocumentManagerException.SESSION_NOT_FOUND,
													   "Session with token " + securityToken + " does not exist");
				Update(securityToken);
				return Mapper.Map<SessionDto>(session);
			}
			catch (DocumentManagerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Update(string securityToken)
		{
			try
			{
				var session = _dao.Read(securityToken);
				if (session == null)
					throw new DocumentManagerException(DocumentManagerException.SESSION_NOT_FOUND,
													   "Session with token " + securityToken + " does not exist");
				session.DateLastAccess = DateTime.Now;
				_dao.Update(session);
			}
			catch (DocumentManagerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Delete(string securityToken)
		{
			try
			{
				var session = _dao.Read(securityToken);
				if (session != null)
					_dao.Delete(session);
			}
			catch (DocumentManagerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void DeleteExpiredSessions(DateTime date)
		{
			try
			{
				_dao.DeleteExpiredSessions(date);
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}

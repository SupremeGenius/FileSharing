using System;
using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;

namespace FileSharing.Services
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
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public SessionDto Read(string securityToken)
		{
			try
			{
				var session = _dao.Read(securityToken);
				if (session == null)
					throw new FileSharingException(FileSharingException.SESSION_NOT_FOUND,
													   "Session with token " + securityToken + " does not exist");
				Update(securityToken);
				return Mapper.Map<SessionDto>(session);
			}
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Update(string securityToken)
		{
			try
			{
				var session = _dao.Read(securityToken);
				if (session == null)
					throw new FileSharingException(FileSharingException.SESSION_NOT_FOUND,
													   "Session with token " + securityToken + " does not exist");
				session.DateLastAccess = DateTime.Now;
				_dao.Update(session);
			}
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
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
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
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
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}

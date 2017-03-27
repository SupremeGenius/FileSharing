using System;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;

namespace DocumentManager.Services
{
	public class UserGroupServices : AbstractServices<UserGroupDao>
	{
		public UserGroupServices() : base(new UserGroupDao()) { }

		public void Create(string securityToken, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				if (_dao.Read(session.IdUser, idGroup) != null)
					throw new DocumentManagerException(DocumentManagerException.USER_GROUP_ALREADY_EXISTS,
												   "User group already exists");

				var userGroup = new UserGroup
				{
					DateInclusionRequest = DateTime.Now,
					IdGroup = idGroup
				};
				userGroup.IdUser = session.IdUser;

				userGroup = _dao.Create(userGroup);
				//TODO Audit
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

		public UserGroupDto Read(string securityToken, long idUser, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var userGroup = _dao.Read(idUser, idGroup);
				if (userGroup == null) return null;
				if (idUser != session.IdUser)
				{
					if (userGroup.IdGroupNavigation.IdAdmin != session.IdUser)
					{
						throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
														   "You do not have permissions to read this user group");
					}
				}
				return Mapper.Map<UserGroupDto>(userGroup);
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

		public void Update(string securityToken, UserGroupDto userGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var userGroupDom = _dao.Read(userGroup.IdUser, userGroup.IdGroup);
				if (userGroupDom == null)
					throw new DocumentManagerException(DocumentManagerException.GROUP_NOT_FOUND,
							"User Group of user " + userGroup.IdUser + " and group " + userGroup.IdGroup + " does not exist");
			
				if (userGroupDom.IdGroupNavigation.IdAdmin != session.IdUser)
					throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
													   "You do not have permissions to update this user group");
				Mapper.Map(userGroup, userGroupDom);
				_dao.Update(userGroupDom);

				string action = "User Group updated:\nOriginal: " + userGroupDom + "\nUpdated: " + userGroup;
				//TODO Audit
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

		public void Delete(string securityToken, long idUser, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var userGroup = _dao.Read(idUser, idGroup);
				if (userGroup != null)
				{
					if (idUser != session.IdUser)
					{
						if (userGroup.IdGroupNavigation.IdAdmin != session.IdUser)
						{
							throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
															   "You do not have permissions to delete this user group");
						}
					}
					_dao.Delete(userGroup);
					string action = "User Group: " + userGroup + " deleted";
					//TODO Audit
				}
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
	}
}

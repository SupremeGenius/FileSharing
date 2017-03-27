using System;
using System.Collections.Generic;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;

namespace DocumentManager.Services
{
	public class GroupServices : AbstractServices<GroupDao>
	{
		public GroupServices() : base(new GroupDao()) { }

		public long Create(string securityToken, string name)
		{
			try
			{
				var session = CheckSession(securityToken);
				var similarName = _dao.QueryByName(name);
				if (similarName.Count > 0 &&
					similarName.Find(g => g.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) != null)
					throw new DocumentManagerException(DocumentManagerException.GROUP_NAME_ALREADY_IN_USE,
													   "Group name already in use");
				var groupDom = new Group
				{
					Name = name,
					IdAdmin = session.IdUser
				};
				groupDom = _dao.Create(groupDom);
				//TODO Audit
				return groupDom.Id;
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

		public GroupDto Read(string securityToken, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(idGroup);
				if (groupDom == null)
					return null;
				
				if (groupDom.IdAdmin != session.IdUser)
				{
					List<UserGroup> users = (List<UserGroup>)groupDom.UserGroup;
					if (users.Find(u => u.IdUser == session.IdUser) == null)
					{
						throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
														   "You do not have permissions to read this group");
					}
				}
				//TODO Audit
				return Mapper.Map<GroupDto>(groupDom);
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

		public void Update(string securityToken, GroupDto groupDto)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(groupDto.Id);
				if (groupDom == null)
					throw new DocumentManagerException(DocumentManagerException.GROUP_NOT_FOUND,
						"Group with id " + groupDto.Id + " does not exist");
				
					if (groupDom.IdAdmin != session.IdUser)
						throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
						                                   "You do not have permissions to update this group");
				string action = "Group updated:\nOriginal: " + groupDom + "\nUpdated: " + groupDto;
				//TODO comprobar que no modifican a un nombre existente
				//TODO Audit
				Mapper.Map(groupDto, groupDom);
				_dao.Update(groupDom);
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

		public void Delete(string securityToken, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(idGroup);
				if (groupDom == null)
					throw new DocumentManagerException(DocumentManagerException.GROUP_NOT_FOUND,
						"Group with id " + idGroup + " does not exist");
				
					if (groupDom.IdAdmin != session.IdUser)
						throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
														   "You do not have permissions to delete this group");
				//TODO Audit
				_dao.Delete(groupDom);
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
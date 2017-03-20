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
				var similarName = _dao.QueryByName(name);
				if (similarName.Count > 0 &&
					similarName.Find(g => g.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) != null)
					throw new DocumentManagerException(DocumentManagerException.GROUP_NAME_ALREADY_IN_USE,
													   "Group name already in use");
				var groupDom = new Group
				{
					Name = name
				};
				using (var sessionService = new SessionServices())
				{
					var session = sessionService.Read(securityToken);
					groupDom.IdAdmin = session.IdUser;
				}
				groupDom = _dao.Create(groupDom);
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
				var groupDom = _dao.Read(idGroup);
				if (groupDom == null)
					return null;
				using (var sessionService = new SessionServices())
				{
					var session = sessionService.Read(securityToken);
					if (groupDom.IdAdmin != session.IdUser)
					{
						List<UserGroup> users = (List<UserGroup>)groupDom.UserGroup;
						if (users.Find(u => u.IdUser == session.IdUser) == null)
						{
							throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
															   "You do not have permissions to read this group");
						}
					}
				}
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
	}
}
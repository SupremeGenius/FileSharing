using System;
using System.Collections.Generic;
using AutoMapper;
using FileStorage.Persistence.Daos;
using FileStorage.Persistence.Models;
using FileStorage.Services.Dtos;
using FileStorage.Services.Exceptions;

namespace FileStorage.Services
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
					throw new FileStorageException(FileStorageException.GROUP_NAME_ALREADY_IN_USE,
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
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
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
						throw new FileStorageException(FileStorageException.UNAUTHORIZED,
														   "You do not have permissions to read this group");
				}
				//TODO Audit
				return Mapper.Map<GroupDto>(groupDom);
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Update(string securityToken, GroupDto groupDto)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(groupDto.Id);
				if (groupDom == null)
					throw new FileStorageException(FileStorageException.GROUP_NOT_FOUND,
						"Group with id " + groupDto.Id + " does not exist");
				
				if (groupDom.IdAdmin != session.IdUser)
					throw new FileStorageException(FileStorageException.UNAUTHORIZED,
					                                   "You do not have permissions to update this group");
				
				var similarName = _dao.QueryByName(groupDto.Name);
				if (similarName.Count > 0 &&
				    similarName.Find(g => g.Name.Equals(groupDto.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
					throw new FileStorageException(FileStorageException.GROUP_NAME_ALREADY_IN_USE,
													   "Group name already in use");
				Mapper.Map(groupDto, groupDom);
				_dao.Update(groupDom);
				//TODO Audit
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Delete(string securityToken, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(idGroup);
				if (groupDom == null)
					throw new FileStorageException(FileStorageException.GROUP_NOT_FOUND,
						"Group with id " + idGroup + " does not exist");
				
					if (groupDom.IdAdmin != session.IdUser)
						throw new FileStorageException(FileStorageException.UNAUTHORIZED,
														   "You do not have permissions to delete this group");
				_dao.Delete(groupDom);
				//TODO Audit
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public List<GroupDto> GetAdministrableGroups(string securityToken)
		{
			try
			{
				var session = CheckSession(securityToken);
				var result = _dao.GetAdministrableGroups(session.IdUser);
				return Mapper.Map<List<GroupDto>>(result);
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}
	}
}
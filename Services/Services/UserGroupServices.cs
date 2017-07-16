using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Persistence.Models.Filters;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace FileSharing.Services
{
    public class UserGroupServices : AbstractServices<UserGroupDao>
	{
		public UserGroupServices() : base(new UserGroupDao()) { }

		public void Create(string securityToken, UserGroupDto userGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
                var existent = Read(securityToken, userGroup.IdUser, userGroup.IdGroup);
                if (existent != null)
                    if (!existent.DateInclusionApproval.HasValue)
                    {
                        throw new FileSharingException(FileSharingException.USER_GROUP_ALREADY_REQUESTED,
                            "The user " + userGroup.IdUser + " has already a request to the group " + userGroup.IdGroup);
                    }
                    else
                    {
                        throw new FileSharingException(FileSharingException.USER_GROUP_ALREADY_MEMBER,
                            "The user " + userGroup.IdUser + " is already member of the group " + userGroup.IdGroup);
                    }
                
                var userGroupDom = Mapper.Map<UserGroup>(userGroup);

                userGroupDom = _dao.Create(userGroupDom);
                Audit(session.IdUser, userGroup.IdGroup.ToString(), typeof(UserGroup).Name, ActionDto.Create, "User add to group: " + userGroupDom);
            }
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
		}

		public UserGroupDto Read(string securityToken, long idUser, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var userGroups = _dao.Query(new UserGroupFilter
                {
                    IdUser = idUser,
                    IdGroup = idGroup
                });
				if (userGroups == null || userGroups.Count == 0) return null;
				return Mapper.Map<UserGroupDto>(userGroups[0]);
			}
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
		}

		public void Update(string securityToken, UserGroupDto userGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var userGroups = _dao.Query(new UserGroupFilter
                {
                    IdUser = userGroup.IdUser,
                    IdGroup = userGroup.IdGroup
                });
                if (userGroups == null || userGroups.Count == 0)
					throw new FileSharingException(FileSharingException.GROUP_NOT_FOUND,
							"User Group of user " + userGroup.IdUser + " and group " + userGroup.IdGroup + " does not exist");

                var userGroupDom = userGroups[0];
                string action = "Update:\r\n" + "-Previous: " + userGroupDom + "\r\n";

                Mapper.Map(userGroup, userGroupDom);
				_dao.Update(userGroupDom);

				 action += "-Updated: " + userGroupDom;
                Audit(session.IdUser, userGroup.IdGroup.ToString(), typeof(UserGroup).Name, ActionDto.Create, action);
            }
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
		}

		public void Delete(string securityToken, long idUser, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
                var userGroups = _dao.Query(new UserGroupFilter
                {
                    IdUser = idUser,
                    IdGroup = idGroup
                });
                if (userGroups != null && userGroups.Count == 1)
				{
					if (idUser != session.IdUser)
					{
                        using (var groupService = new GroupServices())
                        {
                            var group = groupService.Read(securityToken, idGroup);
                            if (group.IdAdmin != session.IdUser)
                            {
                                throw new FileSharingException(FileSharingException.UNAUTHORIZED,
                                                                   "You do not have permissions to delete this user group");
                            }
                        }
					}
					_dao.Delete(userGroups[0]);
					string action = "User Group: " + userGroups[0] + " deleted";
					//TODO Audit
				}
			}
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
        }

        public List<UserGroupDto> Query(string securityToken, UserGroupFilter filter)
        {
            try
            {
                var session = CheckSession(securityToken);
                return Mapper.Map<List<UserGroupDto>>(_dao.Query(filter));
            }
            catch (FileSharingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
            }
        }
	}
}

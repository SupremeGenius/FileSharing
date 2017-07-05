using System;
using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using System.Collections.Generic;
using System.Linq;
using FileSharing.Persistence.Models.Filters;

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
                if (Read(securityToken, userGroup.IdUser, userGroup.IdGroup) != null)
					throw new FileSharingException(FileSharingException.USER_GROUP_ALREADY_EXISTS,
												   "User group already exists");

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
                    IdUser = session.IdUser,
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

				Mapper.Map(userGroup, userGroups[0]);
				_dao.Update(userGroups[0]);

				string action = "Update:\r\n" + "-Previous: " + userGroups + "\r\n" + "-Updated: " + userGroup;
				//TODO Audit
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

        public List<UserDto> GetUsersOfGroup(string securityToken, long idGroup)
        {
            try
            {
                var session = CheckSession(securityToken);
                List<UserDto> result = new List<UserDto>();
                using (var userService = new UserServices())
                {
                    var userGroups = _dao.Query(new UserGroupFilter
                    {
                        IdGroup = idGroup,
                        DateInclusionApprovalTo = DateTime.Now
                    });
                    foreach (var user in 
                        userGroups.Where(x => x.DateInclusionApproval.HasValue).Select(x => userService.Read(securityToken, x.IdUser)))
                    {
                        result.Add(user);
                    }
                }
                return result;
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

        public int NumOfMembersOfAGroup(string securityToken, long idGroup)
        {
            try
            {
                var session = CheckSession(securityToken);
                var userGroups = _dao.Query(new UserGroupFilter
                {
                    IdUser = session.IdUser,
                    IdGroup = idGroup,
                    DateInclusionApprovalTo = DateTime.Now
                });
                if (userGroups == null) return 0;
                return userGroups.Where(x => x.DateInclusionApproval.HasValue).ToList().Count;
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

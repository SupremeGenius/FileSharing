using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Persistence.Models.Filters;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSharing.Services
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
					throw new FileSharingException(FileSharingException.GROUP_NAME_ALREADY_IN_USE,
													   "Group name already in use");
				var groupDom = new Group
				{
					Name = name,
					IdAdmin = session.IdUser
				};
				groupDom = _dao.Create(groupDom);
                Audit(session.IdUser, groupDom.Id.ToString(), typeof(Group).Name, ActionDto.Create, "Group created: " + groupDom);

                using (var userGroupService = new UserGroupServices())
                {
                    var userGroup = new UserGroupDto
                    {
                        IdUser = session.IdUser,
                        IdGroup = groupDom.Id,
                        DateInclusionRequest = DateTime.Now,
                        DateInclusionApproval = DateTime.Now
                    };
                    userGroupService.Create(securityToken, userGroup);
                }
                return groupDom.Id;
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

		public GroupDto Read(string securityToken, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(idGroup);
				return Mapper.Map<GroupDto>(groupDom);
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

		public void Update(string securityToken, GroupDto groupDto)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(groupDto.Id);
				if (groupDom == null)
					throw new FileSharingException(FileSharingException.GROUP_NOT_FOUND,
						"Group with id " + groupDto.Id + " does not exist");
				
				if (groupDom.IdAdmin != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
					                                   "You do not have permissions to update this group");
				
				var similarName = _dao.QueryByName(groupDto.Name);
				if (similarName.Count > 0 &&
				    similarName.Find(g => g.Name.Equals(groupDto.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
					throw new FileSharingException(FileSharingException.GROUP_NAME_ALREADY_IN_USE,
													   "Group name already in use");
				Mapper.Map(groupDto, groupDom);
				_dao.Update(groupDom);
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

		public void Delete(string securityToken, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var groupDom = _dao.Read(idGroup);
				if (groupDom == null)
					throw new FileSharingException(FileSharingException.GROUP_NOT_FOUND,
						"Group with id " + idGroup + " does not exist");
				
					if (groupDom.IdAdmin != session.IdUser)
						throw new FileSharingException(FileSharingException.UNAUTHORIZED,
														   "You do not have permissions to delete this group");
				_dao.Delete(groupDom);
                Audit(session.IdUser, groupDom.Id.ToString(), typeof(Group).Name, ActionDto.Delete, "Group deleted: " + groupDom);
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

		public List<GroupDto> GetAdministrableGroups(string securityToken)
		{
			try
			{
				var session = CheckSession(securityToken);
				var result = _dao.GetAdministrableGroups(session.IdUser);
				return Mapper.Map<List<GroupDto>>(result);
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

        public List<GroupDetailsDto> GetGroupsOfUser(string securityToken)
        {
            try
            {
                var session = CheckSession(securityToken);
                List<GroupDetailsDto> result = new List<GroupDetailsDto>();
                using (var userGroupService = new UserGroupServices())
                {
                    var userGroups = userGroupService.Query(securityToken, new UserGroupFilter
                    {
                        IdUser = session.IdUser,
                        DateInclusionApprovalTo = DateTime.Now
                    });
                    if (userGroups == null || userGroups.Count == 0) return result;

                    foreach (var group in userGroups.Select(x => _dao.Read(x.IdGroup)))
                    {
                        var groupDetails = Mapper.Map<GroupDetailsDto>(group);
                        groupDetails.IsAdministrable = group.IdAdmin == session.IdUser;
                        groupDetails.NumOfMembers = userGroupService.NumOfMembersOfAGroup(securityToken, group.Id);
                        result.Add(groupDetails);
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

        public GroupDetailsExtendedDto GetGroupDetails(string securityToken, long idGroup)
        {
            try
            {
                var session = CheckSession(securityToken);
                var group = _dao.Read(idGroup);
                var result = Mapper.Map<GroupDetailsExtendedDto>(group);
                result.IsAdministrable = group.IdAdmin == session.IdUser;

                using (var userGroupService = new UserGroupServices())
                using (var fileService = new FileServices())
                {
                    result.Members = userGroupService.GetUsersOfGroup(securityToken, result.Id);
                    result.Files = fileService.GetFilesByGroup(securityToken, result.Id);
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
    }
}
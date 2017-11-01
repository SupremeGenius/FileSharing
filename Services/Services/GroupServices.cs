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
				var similarName = _dao.QueryByName(name,0 ,0);
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
				
                if (!groupDom.Name.Equals(groupDto.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    var similarName = _dao.QueryByName(groupDto.Name, 0, 0);
                    if (similarName.Count > 0 &&
                        similarName.Find(g => g.Name.Equals(groupDto.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
                        throw new FileSharingException(FileSharingException.GROUP_NAME_ALREADY_IN_USE,
                                                           "Group name already in use");
                }

                string action = "Update:\r\n" + "-Previous: " + groupDom + "\r\n";

                Mapper.Map(groupDto, groupDom);
				_dao.Update(groupDom);

                action += "-Updated: " + groupDom;

                Audit(session.IdUser, groupDom.Id.ToString(), typeof(Group).Name, ActionDto.Delete, action);
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
				var groupDom = _dao.ReadFullGroup(idGroup);
				if (groupDom == null)
					throw new FileSharingException(FileSharingException.GROUP_NOT_FOUND,
						"Group with id " + idGroup + " does not exist");
				
				if (groupDom.IdAdmin != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
														"You do not have permissions to delete this group.");
                if (groupDom.Users.Count > 1)
                    throw new FileSharingException(FileSharingException.CANNOT_DELETE_GROUP_WITH_MEMBERS,
                                                        "You cannot delete a group with members.");

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
                var groups = _dao.GetByUser(session.IdUser);
                List<GroupDetailsDto> result = Mapper.Map<List<GroupDetailsDto>>(groups);

                for(var i = 0; i < groups.Count; i++)
                {
                    result[i].IsAdministrable = groups[i].IdAdmin == session.IdUser;
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

        public GroupsAndRequestsDto GetGroupsAndRequestsOfUser(string securityToken)
        {
            try
            {
                var session = CheckSession(securityToken);
                var result = new GroupsAndRequestsDto();
                var groups = _dao.GetByUser(session.IdUser);
                var groupsResult = Mapper.Map<List<GroupDetailsDto>>(groups);

                for (var i = 0; i < groups.Count; i++)
                {
                    groupsResult[i].IsAdministrable = groups[i].IdAdmin == session.IdUser;
                }
                result.Groups = groupsResult;
                using (var userGroupServices = new UserGroupServices())
                {
                    result.Requests = userGroupServices.GetRequestsOfUser(securityToken);
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
                var group = _dao.ReadFullGroup(idGroup);

                if (group.Users.Where(x => x.IdUser == session.IdUser).FirstOrDefault() == null)
                    throw new FileSharingException(FileSharingException.UNAUTHORIZED,
                                                        "You do not have permissions to read this group.");
                var result = Mapper.Map<GroupDetailsExtendedDto>(group);

                result.IsAdministrable = group.IdAdmin == session.IdUser;

                var self = result.Members.Where(x => x.Id == session.IdUser).FirstOrDefault();
                if (self != null)
                    result.Members.Remove(self);

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

        public List<GroupDetailsDto> QueryByName(string securityToken, string name, int rowQty, int page)
        {
            try
            {
                var session = CheckSession(securityToken);
                var groups = _dao.QueryByName(name, rowQty, page);
                return Mapper.Map<List<GroupDetailsDto>>(groups);
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
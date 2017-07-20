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
				var userGroup = _dao.Read(idUser, idGroup);
				return Mapper.Map<UserGroupDto>(userGroup);
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

		public void Accept(string securityToken, long idUser, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
				var userGroup = _dao.Read(idUser, idGroup);
                if (userGroup == null)
					throw new FileSharingException(FileSharingException.GROUP_NOT_FOUND,
							"User Group of user " + idUser + " and group " + idGroup + " does not exist");
                
                userGroup.DateInclusionApproval = DateTime.Now;
				_dao.Update(userGroup);
                
                Audit(session.IdUser, userGroup.IdGroup.ToString(), typeof(UserGroup).Name, ActionDto.Update, "-Accepted: " + userGroup);
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

		public void Reject(string securityToken, long idUser, long idGroup)
		{
			try
			{
				var session = CheckSession(securityToken);
                var userGroup = _dao.ReadFull(idUser, idGroup);
                if (userGroup != null)
				{
					if (idUser != session.IdUser
                        && userGroup.Group.IdAdmin != session.IdUser)
					{
                        throw new FileSharingException(FileSharingException.UNAUTHORIZED,
                            "You do not have permissions to delete this user group");

					}
					_dao.Delete(userGroup);
                    Audit(session.IdUser, idGroup.ToString(), typeof(UserGroup).Name, ActionDto.Delete, "User Group: " + userGroup + " deleted");
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

        public List<UserGroupDetailsDto> GetRequestsOfUser(string securityToken)
        {
            try
            {
                var session = CheckSession(securityToken);
                var requests = _dao.GetRequestsByUser(session.IdUser);
                return Mapper.Map<List<UserGroupDetailsDto>>(requests);
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

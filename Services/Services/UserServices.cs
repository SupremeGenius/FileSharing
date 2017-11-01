using System;
using System.Security.Cryptography;
using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
namespace FileSharing.Services
{
	public class UserServices : AbstractServices<UserDao>
	{
		public UserServices() : base(new UserDao()) { }

		public string Register(UserRegistrationDto user)
		{
			try
            {
                if (user.Username.Length < 6)
                    throw new FileSharingException(FileSharingException.USERNAME_FIELD_LENGTH, "Username must have 6 characters at least");
                if (user.Password.Length < 6)
                    throw new FileSharingException(FileSharingException.PASSWORD_FIELD_LENGTH, "Password must have 6 characters at least");
                if (user.Password != user.ConfirmPassword)
                    throw new FileSharingException(FileSharingException.PASSWORD_NOT_MATCHING, "The password and confirmation password do not match");
                var userDom = Mapper.Map<User>(user);
                ValidateUser(userDom);
				if (_dao.ReadByLogin(userDom.Login) != null)
					throw new FileSharingException(FileSharingException.LOGIN_ALREADY_IN_USE, "Login already in use");

				userDom.Password = EncryptPassword(user.Password);
				userDom = _dao.Create(userDom);

				Audit(userDom.Id, userDom.Id.ToString(), typeof(User).Name, ActionDto.Create, "User registered: " + userDom);

                using (var _sessionServices = new SessionServices())
                    return _sessionServices.Create(userDom.Id);
            }
			catch (FileSharingException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
		}

		public UserDto Read(string securityToken)
		{
			try
			{
				var session = CheckSession(securityToken);
				var user = _dao.Read(session.IdUser);
				return Mapper.Map<UserDto>(user);
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
        }

        public UserDto Read(string securityToken, long idUser)
        {
            try
            {
                var session = CheckSession(securityToken);
                var user = _dao.Read(idUser);
                return Mapper.Map<UserDto>(user);
            }
            catch (Exception e)
            {
                throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
            }
        }

        public void Update(string securityToken, UserDto user)
		{
			try
			{
				var session = CheckSession(securityToken);
				ValidateUser(Mapper.Map<User>(user));
				User userDom = _dao.Read(user.Id);
				if (userDom == null)
					throw new FileSharingException(FileSharingException.USER_NOT_FOUND,
						"User with id " + user.Id + " does not exist");
				if (userDom.Id != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
													   "You do not have permissions to update this user");
				
				if (userDom.Login != user.Login)
				{
					if (_dao.ReadByLogin(user.Login) != null)
						throw new FileSharingException(FileSharingException.LOGIN_ALREADY_IN_USE, "Login already in use");
					userDom.Login = user.Login;
				}
				var password = userDom.Password;
				var oldUser = userDom.ToString();

				Mapper.Map(user, userDom);
				userDom.Password = password; //Password not allowed to be modified by this method

				Audit(userDom.Id, userDom.Id.ToString(), typeof(User).Name, ActionDto.Update,
                      "User updated:\r\n" + "-Previous: " + oldUser + "\r\n" + "-Updated: " + userDom);

				_dao.Update(userDom);
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

		public void Delete(string securityToken, string password)
		{
			try
			{
				var session = CheckSession(securityToken);
				var user = _dao.Read(session.IdUser);
				if (user.Password != EncryptPassword(password))
					throw new FileSharingException(FileSharingException.INVALID_CREDENTIALS,
													   "The password is invalid");

				using (var groupServices = new GroupServices())
				{
					var groups = groupServices.GetAdministrableGroups(securityToken);
					if (groups != null && groups.Count > 0)
						throw new FileSharingException(FileSharingException.USER_CANNOT_BE_REMOVED,
													   "The user cannot be removed because he is group admin");
				}

				using (var fileService = new FileServices())
				{
					foreach (var doc in user.Files)
					{
                        fileService.Delete(securityToken, doc.Id);
					}
				}

				using (var folderServices = new FolderServices())
				{
					foreach (var folder in user.Folders)
					{
						folderServices.Delete(securityToken, folder.Id);
					}
				}

				_dao.Delete(user);

				Audit(user.Id, user.Id.ToString(), typeof(User).Name, ActionDto.Delete, "User deleted: " + user);
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
		}

		public string Login(UserLoginDto login)
		{
			try
			{
				var userDom = _dao.ReadByLogin(login.Username);
				if (userDom == null || userDom.Password != EncryptPassword(login.Password))
					throw new FileSharingException(FileSharingException.INVALID_CREDENTIALS,
					                                   "The username or the password is not valid");
                using (var _sessionServices = new SessionServices())
                    return _sessionServices.Create(userDom.Id);
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

		public void Logout(string securityToken)
		{
			try
            {
                using (var _sessionServices = new SessionServices())
                    _sessionServices.Delete(securityToken);
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
			}
		}

		public void ChangePassword(string securityToken, string oldPassword, string newPassword)
		{
			try
			{
				var session = CheckSession(securityToken);
				var user = _dao.Read(session.IdUser);
				if (user.Password != EncryptPassword(oldPassword))
					throw new FileSharingException(FileSharingException.INVALID_CREDENTIALS,
													   "The password is invalid");
				user.Password = EncryptPassword(newPassword);
				_dao.Update(user);

				Audit(user.Id, user.Id.ToString(), typeof(User).Name, ActionDto.Update, "User password changed: " + user.Login);
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

		#region Private Members

		string EncryptPassword(string password)
		{
			byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
			using (var hash = SHA256.Create())
			{
				data = hash.ComputeHash(data);
			}
			return System.Text.Encoding.ASCII.GetString(data);
		}

		void ValidateUser(User user)
		{
			if (user == null)
				throw new FileSharingException(FileSharingException.NULL_VALUE, "User cannot be null");
			if (string.IsNullOrWhiteSpace(user.Login))
				throw new FileSharingException(FileSharingException.NULL_VALUE, "Login cannot be null");
			if (string.IsNullOrWhiteSpace(user.Password))
				throw new FileSharingException(FileSharingException.NULL_VALUE, "Password cannot be null");
			if (string.IsNullOrWhiteSpace(user.FirstName))
				throw new FileSharingException(FileSharingException.NULL_VALUE, "FirstName cannot be null");
			if (string.IsNullOrWhiteSpace(user.LastName))
				throw new FileSharingException(FileSharingException.NULL_VALUE, "LastName cannot be null");
		}

		#endregion
	}
}

using System;
using System.Security.Cryptography;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;

namespace DocumentManager.Services
{
	public class UserServices : AbstractServices<UserDao>
	{
		public UserServices() : base(new UserDao()) { }

		public long Register(UserDto user)
		{
			try
			{
				ValidateUser(user);
				if (_dao.ReadByLogin(user.Login) != null)
					throw new DocumentManagerException(DocumentManagerException.LOGIN_ALREADY_IN_USE, "Login already in use");

				var userDom = Mapper.Map<User>(user);
				userDom.Password = EncryptPassword(user.Password);
				userDom = _dao.Create(userDom);

				Audit(userDom.Id.ToString(), typeof(User).Name, "Register user: " + userDom, userDom.Id);

				return userDom.Id;
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

		public UserDto Read(string securityToken)
		{
			try
			{
				var session = CheckSession(securityToken);
				var user = _dao.Read(session.IdUser);
				if (user == null)
					return null;
				return Mapper.Map<UserDto>(user);
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Update(string securityToken, UserDto user)
		{
			try
			{
				var session = CheckSession(securityToken);
				ValidateUser(user);
				User userDom = _dao.Read(user.Id);
				if (userDom == null)
					throw new DocumentManagerException(DocumentManagerException.USER_NOT_FOUND,
						"User with id " + user.Id + " does not exist");
				if (userDom.Id != session.IdUser)
					throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
													   "You do not have permissions to update this user");
				
				if (userDom.Login != user.Login)
				{
					if (_dao.ReadByLogin(user.Login) != null)
						throw new DocumentManagerException(DocumentManagerException.LOGIN_ALREADY_IN_USE, "Login already in use");
					userDom.Login = user.Login;
				}
				var password = userDom.Password;
				var oldUser = userDom;

				Mapper.Map(user, userDom);
				userDom.Password = password; //Password not allowed to be modified by this method

				Audit(userDom.Id.ToString(), typeof(User).Name,
				      "Update:\n-Old user: " + oldUser + "\n-New user: " + userDom, userDom.Id);

				_dao.Update(userDom);
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

		public void Delete(string securityToken, string password)
		{
			try
			{
				var session = CheckSession(securityToken);
				var user = _dao.Read(session.IdUser);
				if (user.Password != EncryptPassword(password))
					throw new DocumentManagerException(DocumentManagerException.INVALID_CREDENTIALS,
													   "The password is invalid");

                //TODO comprobar si es admin de algún grupo, si es así, pedir que lo transfiera antes

				using (var documentServices = new DocumentServices())
				{
					foreach (var doc in user.Document)
					{
						documentServices.Delete(securityToken, doc.Id);
					}
				}

				using (var folderServices = new FolderServices())
				{
					foreach (var folder in user.Folder)
					{
						folderServices.Delete(securityToken, folder.Id);
					}
				}

				//TODO hacer Session y UserGroup que se eliminen en casacada en DB

				_dao.Delete(user);
				//TODO cambiar audit para que permita borrar el user sin borrar audits
				Audit(user.Id.ToString(), typeof(User).Name, "Deleted user: " + user, user.Id);
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public string Login(string login, string password)
		{
			try
			{
				var userDom = _dao.ReadByLogin(login);
				if (userDom == null || userDom.Password != EncryptPassword(password))
					throw new DocumentManagerException(DocumentManagerException.INVALID_CREDENTIALS,
					                                   "The login or password is invalid");
                using (var _sessionServices = new SessionServices())
                    return _sessionServices.Create(userDom.Id);
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

		public void Logout(string securityToken)
		{
			try
            {
                using (var _sessionServices = new SessionServices())
                    _sessionServices.Delete(securityToken);
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void ChangePassword(string securityToken, string oldPassword, string newPassword)
		{
			try
			{
				var session = CheckSession(securityToken);
				var user = _dao.Read(session.IdUser);
				if (user.Password != EncryptPassword(oldPassword))
					throw new DocumentManagerException(DocumentManagerException.INVALID_CREDENTIALS,
													   "The password is invalid");
				user.Password = EncryptPassword(newPassword);
				_dao.Update(user);

				Audit(user.Id.ToString(), typeof(User).Name, "Changed password of user: " + user.Login, user.Id);
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

		void ValidateUser(UserDto user)
		{
			if (user == null)
				throw new DocumentManagerException(DocumentManagerException.NULL_VALUE, "User cannot be null");
			if (String.IsNullOrWhiteSpace(user.Login))
				throw new DocumentManagerException(DocumentManagerException.NULL_VALUE, "Login cannot be null");
			if (String.IsNullOrWhiteSpace(user.Password))
				throw new DocumentManagerException(DocumentManagerException.NULL_VALUE, "Password cannot be null");
			if (String.IsNullOrWhiteSpace(user.FirstName))
				throw new DocumentManagerException(DocumentManagerException.NULL_VALUE, "FirstName cannot be null");
			if (String.IsNullOrWhiteSpace(user.LastName))
				throw new DocumentManagerException(DocumentManagerException.NULL_VALUE, "LastName cannot be null");
		}

		#endregion
	}
}

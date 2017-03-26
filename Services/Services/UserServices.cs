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
				Audit("New user created", userDom.Id);
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

		public UserDto Read(long idUser)
		{
			try
			{
				var user = _dao.Read(idUser);
				if (user == null)
					return null;
				return Mapper.Map<UserDto>(user);
			}
			catch (Exception e)
			{
				throw new DocumentManagerException(DocumentManagerException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Update(UserDto user)
		{
			try
			{
				ValidateUser(user);
				User userDom = _dao.Read(user.Id);
				if (userDom == null)
					throw new DocumentManagerException(DocumentManagerException.USER_NOT_FOUND,
						"User with id " + user.Id + " does not exist");

				if (userDom.Login != user.Login)
				{
					if (_dao.ReadByLogin(user.Login) != null)
						throw new DocumentManagerException(DocumentManagerException.LOGIN_ALREADY_IN_USE, "Login already in use");
					userDom.Login = user.Login;
				}
				var password = userDom.Password;
				Mapper.Map(user, userDom);
				userDom.Password = password; //Password not allowed to be modified by this method

				Audit("User updated", userDom.Id);
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

		public void Delete(long idUser)
		{
			try
			{
				var user = _dao.Read(idUser);
				if (user != null)
				{
					_dao.Delete(user);
					Audit("User deleted", idUser);
				}
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
					throw new DocumentManagerException(DocumentManagerException.INVALID_CREDENTIALS, "The login or password is invalid");
				using (var session = new SessionServices())
				{
					return session.Create(userDom.Id);
				}
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

		void Audit(string action, long id)
		{
			try
			{
				var audit = new AuditDto
				{
					Action = action,
					IdObject = id,
					IdUser = id,
					Object = typeof(User).Name
				};
				using (var auditService = new AuditServices())
				{
					auditService.Create(audit);
				}
			}
			catch (Exception)
			{
				//TODO Implement a log.
			}
		}

		#endregion
	}
}

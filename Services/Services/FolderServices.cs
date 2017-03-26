using System;
using System.Collections.Generic;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;

namespace DocumentManager.Services.Services
{
	public class FolderServices : AbstractServices<FolderDao>
	{
		public FolderServices() : base(new FolderDao()) { }

		public long Create(string securityToken, FolderDto folder)
		{
			try
			{
				var similarName = _dao.GetByNameAndFolderRoot(folder.Name, folder.IdFolderRoot);
				if (similarName != null)
					throw new DocumentManagerException(DocumentManagerException.FOLDER_ALREADY_EXISTS,
													   "You already have a folder with this name in this directory");
				var folderDom = Mapper.Map<Folder>(folder);
				using (var sessionService = new SessionServices())
				{
					var session = sessionService.Read(securityToken);
					folderDom.IdUser = session.IdUser;
				}
				folderDom = _dao.Create(folderDom);

				//TODO Audit
				return folderDom.Id;
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

		public void Update(string securityToken, FolderDto folder)
		{
			try
			{
				var folderDom = _dao.Read(folder.Id);
				if (folderDom == null)
					throw new DocumentManagerException(DocumentManagerException.FOLDER_NOT_FOUND,
						 "Folder with id " + folder.Id + " does not exist");
				using (var sessionService = new SessionServices())
				{
					var session = sessionService.Read(securityToken);
					if (folderDom.IdUser != session.IdUser)
						throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
														   "You do not have permissions to update this folder");
				}
				var similarName = _dao.GetByNameAndFolderRoot(folder.Name, folder.IdFolderRoot);
				if (similarName != null)
					throw new DocumentManagerException(DocumentManagerException.FOLDER_ALREADY_EXISTS,
													   "You already have a folder with this name in this directory");
				//TODO Audit
				Mapper.Map(folder, folderDom);
				_dao.Update(folderDom);
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

		public void Delete(string securityToken, long idFolder)
		{
			try
			{
				var folderDom = _dao.Read(idFolder);
				if (folderDom == null)
					throw new DocumentManagerException(DocumentManagerException.FOLDER_NOT_FOUND,
						 "Folder with id " + idFolder + " does not exist");
				using (var sessionService = new SessionServices())
				{
					var session = sessionService.Read(securityToken);
					if (folderDom.IdUser != session.IdUser)
						throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
														   "You do not have permissions to update this folder");
				}
				//TODO Audit
				_dao.Delete(folderDom);
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

		public List<FolderDto> GetUserFolders(string securityToken)
		{
			try
			{
				using (var sessionService = new SessionServices())
				{
					var session = sessionService.Read(securityToken);
					var folders = _dao.GetByUser(session.IdUser);
					return Mapper.Map<List<FolderDto>>(folders);
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
	}
}

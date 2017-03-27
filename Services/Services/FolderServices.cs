using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;
using Microsoft.Extensions.Configuration;

namespace DocumentManager.Services.Services
{
	public class FolderServices : AbstractServices<FolderDao>
	{
		readonly string repoPath;

		public FolderServices() : base(new FolderDao())
		{
			var configurationBuilder = new ConfigurationBuilder()
				 .AddJsonFile("repo_settings.json");
			var configuration = configurationBuilder.Build();

			repoPath = $"{configuration["RepositoryPath"]}"; }

		public long Create(string securityToken, FolderDto folder)
		{
			try
			{
				var similarName = _dao.GetByNameAndFolderRoot(folder.Name, folder.IdFolderRoot);
				if (similarName != null)
					throw new DocumentManagerException(DocumentManagerException.FOLDER_ALREADY_EXISTS,
													   "You already have a folder with this name in this directory");
				if (folder.IdFolderRoot.HasValue)
				{
					var rootFolder = _dao.Read(folder.IdFolderRoot.Value);
					if (rootFolder == null)
						throw new DocumentManagerException(DocumentManagerException.FOLDER_NOT_FOUND,
														   "Folder with id " + folder.Id + " does not exist");
				}
				var folderDom = Mapper.Map<Folder>(folder);
				using (var sessionService = new SessionServices())
				{
					var session = sessionService.Read(securityToken);
					folderDom.IdUser = session.IdUser;
				}
				folderDom = _dao.Create(folderDom);

				Directory.CreateDirectory(GetFullPath(folderDom));
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
				string previousPath = GetFullPath(folderDom);
				Mapper.Map(folder, folderDom);
				_dao.Update(folderDom);
				Directory.Move(previousPath, GetFullPath(folderDom));
				Directory.Delete(previousPath);
				//TODO Audit

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
				string path = GetFullPath(folderDom);
				_dao.Delete(folderDom);
				Directory.Delete(path);
				//TODO Audit
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

		public string GetFullPath(long idFolder)
		{
			var folder = _dao.Read(idFolder);
			if (folder == null)
				throw new DocumentManagerException(DocumentManagerException.FOLDER_NOT_FOUND,
					 "Folder with id " + idFolder + " does not exist");
			return GetFullPath(_dao.Read(idFolder));
		}

		#region Private Methods

		string GetFullPath(Folder folder)
		{
			var _folder = folder;
			string path = "";
			while (_folder.IdFolderRoot.HasValue)
			{
				path = folder.Name + "/" + path;
				_folder = _folder.IdFolderRootNavigation;
			}
			path = _folder.Name + "/" + path;
			return repoPath + path;
		}

		#endregion
	}
}

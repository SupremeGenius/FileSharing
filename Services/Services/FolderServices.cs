using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using FileStorage.Persistence.Daos;
using FileStorage.Persistence.Models;
using FileStorage.Services.Dtos;
using FileStorage.Services.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FileStorage.Services
{
	public class FolderServices : AbstractServices<FolderDao>
	{
		readonly string repoPath;

		public FolderServices() : base(new FolderDao())
		{
			var configurationBuilder = new ConfigurationBuilder()
				 .AddJsonFile("repo_settings.json");
			var configuration = configurationBuilder.Build();

			repoPath = $"{configuration["RepositoryPath"]}";
		}

		public long Create(string securityToken, FolderDto folder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var similarName = _dao.GetByNameAndFolderRoot(folder.Name, folder.IdFolderRoot);
				if (similarName != null)
					throw new FileStorageException(FileStorageException.FOLDER_ALREADY_EXISTS,
													   "You already have a folder with this name in this directory");
				if (folder.IdFolderRoot.HasValue)
				{
					var rootFolder = _dao.Read(folder.IdFolderRoot.Value);
					if (rootFolder == null)
						throw new FileStorageException(FileStorageException.FOLDER_NOT_FOUND,
														   "Folder with id " + folder.Id + " does not exist");
				}
				var folderDom = Mapper.Map<Folder>(folder);
				folderDom.IdUser = session.IdUser;

				Directory.CreateDirectory(GetFullPath(folderDom));

				folderDom = _dao.Create(folderDom);
				//TODO Audit
				return folderDom.Id;
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Update(string securityToken, FolderDto folder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var folderDom = _dao.Read(folder.Id);
				if (folderDom == null)
					throw new FileStorageException(FileStorageException.FOLDER_NOT_FOUND,
						 "Folder with id " + folder.Id + " does not exist");
				
				if (folderDom.IdUser != session.IdUser)
					throw new FileStorageException(FileStorageException.UNAUTHORIZED,
													   "You do not have permissions to update this folder");
				
				var similarName = _dao.GetByNameAndFolderRoot(folder.Name, folder.IdFolderRoot);
				if (similarName != null)
					throw new FileStorageException(FileStorageException.FOLDER_ALREADY_EXISTS,
													   "You already have a folder with this name in this directory");
				string previousPath = GetFullPath(folderDom);
				Mapper.Map(folder, folderDom);

				Directory.Move(previousPath, GetFullPath(folderDom));
				Directory.Delete(previousPath);

				_dao.Update(folderDom);
				//TODO Audit
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Delete(string securityToken, long idFolder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var folderDom = _dao.Read(idFolder);
				if (folderDom == null)
					throw new FileStorageException(FileStorageException.FOLDER_NOT_FOUND,
						 "Folder with id " + idFolder + " does not exist");
				
				if (folderDom.IdUser != session.IdUser)
					throw new FileStorageException(FileStorageException.UNAUTHORIZED,
													   "You do not have permissions to update this folder");
				
				string path = GetFullPath(folderDom);
				Directory.Delete(path);

				_dao.Delete(folderDom);
				//TODO Audit
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public List<FolderDto> GetFoldersInFolder(string securityToken, long? idFolder)
		{
			try
			{
				var session = CheckSession(securityToken);
				Folder folder;
				if (!idFolder.HasValue)
				{
					folder = _dao.GetUserRootFolder(session.IdUser);
				}
				else
				{
					folder = _dao.Read(idFolder.Value);
				}

				if (folder == null || folder.InverseIdFolderRootNavigation == null
				    || folder.InverseIdFolderRootNavigation.Count == 0)
					return new List<FolderDto>();
				
				List<Folder> folders = new List<Folder>();
				folders.AddRange(folder.InverseIdFolderRootNavigation);

				return Mapper.Map<List<FolderDto>>(folders);
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public string GetFullPath(string securityToken, long? idFolder)
		{
			try
			{
				var session = CheckSession(securityToken);
				string path = GetUserRootFolder(session.IdUser) + Path.DirectorySeparatorChar.ToString();

				if (idFolder.HasValue)
				{
					var folder = _dao.Read(idFolder.Value);
					if (folder == null)
						throw new FileStorageException(FileStorageException.FOLDER_NOT_FOUND,
							 "Folder with id " + idFolder.Value + " does not exist");
					path += GetFullPath(_dao.Read(idFolder.Value)) + Path.DirectorySeparatorChar.ToString();
				}

				return path;
			}
			catch (FileStorageException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileStorageException(FileStorageException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		#region Private Methods

		string GetFullPath(Folder folder)
		{
			if (folder.IdFolderRoot.HasValue)
				return GetFullPath(folder.IdFolderRootNavigation) + Path.DirectorySeparatorChar.ToString() + folder.Name;
					
			return folder.Name;
		}

		string GetUserRootFolder(long idUser)
		{
			string path = repoPath + Path.DirectorySeparatorChar.ToString() + idUser;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}

		#endregion
	}
}

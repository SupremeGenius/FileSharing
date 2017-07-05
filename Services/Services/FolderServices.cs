using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FileSharing.Services
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
					throw new FileSharingException(FileSharingException.FOLDER_ALREADY_EXISTS,
													   "You already have a folder with this name in this directory");
				if (folder.IdFolderRoot.HasValue)
				{
					var rootFolder = _dao.Read(folder.IdFolderRoot.Value);
					if (rootFolder == null)
						throw new FileSharingException(FileSharingException.FOLDER_NOT_FOUND,
														   "Folder with id " + folder.Id + " does not exist");
				}
				var folderDom = Mapper.Map<Folder>(folder);
				folderDom.IdUser = session.IdUser;

				Directory.CreateDirectory(GetFullPath(folderDom));

				folderDom = _dao.Create(folderDom);
                Audit(session.IdUser, folderDom.Id.ToString(), typeof(Folder).Name, ActionDto.Create, "Folder created: " + folderDom);
                return folderDom.Id;
			}
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

        public FolderDto Read(string securityToken, long idFolder)
        {
            try
            {
                var session = CheckSession(securityToken);
                var folder = _dao.Read(idFolder);
                if (folder == null)
                    throw new FileSharingException(FileSharingException.FOLDER_NOT_FOUND,
                         "Folder with id " + idFolder + " does not exist");

                if (folder.IdUser != session.IdUser)
                    throw new FileSharingException(FileSharingException.UNAUTHORIZED,
                                                       "You do not have permissions to update this folder");
                return Mapper.Map<FolderDto>(folder);
            }
            catch (FileSharingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
            }
        }

		public void Update(string securityToken, FolderDto folder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var folderDom = _dao.Read(folder.Id);
				if (folderDom == null)
					throw new FileSharingException(FileSharingException.FOLDER_NOT_FOUND,
						 "Folder with id " + folder.Id + " does not exist");
				
				if (folderDom.IdUser != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
													   "You do not have permissions to update this folder");
				
				var similarName = _dao.GetByNameAndFolderRoot(folder.Name, folder.IdFolderRoot);
				if (similarName != null)
					throw new FileSharingException(FileSharingException.FOLDER_ALREADY_EXISTS,
													   "You already have a folder with this name in this directory");
				string previousPath = GetFullPath(folderDom);
				Mapper.Map(folder, folderDom);

				Directory.Move(previousPath, GetFullPath(folderDom));
				Directory.Delete(previousPath);

				_dao.Update(folderDom);
				//TODO Audit
			}
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		public void Delete(string securityToken, long idFolder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var folderDom = _dao.Read(idFolder);
				if (folderDom == null)
					throw new FileSharingException(FileSharingException.FOLDER_NOT_FOUND,
						 "Folder with id " + idFolder + " does not exist");
				
				if (folderDom.IdUser != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
													   "You do not have permissions to update this folder");

                var folders = GetFoldersInFolder(session.IdUser, idFolder);
                foreach(var folder in folders){
                    Delete(securityToken, folder.Id);
                }
				
				_dao.Delete(folderDom);
                Audit(session.IdUser, folderDom.Id.ToString(), typeof(Folder).Name, ActionDto.Delete, "Folder deletes: " + folderDom);
            }
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

        public FolderDetailsDto GetFolderDetails(string securityToken, long? idFolder)
        {
            try
            {
                var session = CheckSession(securityToken);
                FolderDetailsDto result = new FolderDetailsDto();
                if (idFolder.HasValue) result = Mapper.Map<FolderDetailsDto>(Read(securityToken, idFolder.Value));
                result.Folders = GetFoldersInFolder(session.IdUser, idFolder);
                using (var documentService = new DocumentServices())
                {
                    result.Documents = documentService.GetDocumentsInFolder(securityToken, idFolder);
                }
                return result;
            }
            catch (FileSharingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
            }
        }

		public string GetFullPath(string securityToken, long? idFolder)
		{
			try
			{
				var session = CheckSession(securityToken);;

				if (idFolder.HasValue)
				{
					var folder = _dao.Read(idFolder.Value);
					if (folder == null)
						throw new FileSharingException(FileSharingException.FOLDER_NOT_FOUND,
							 "Folder with id " + idFolder.Value + " does not exist");
					return GetFullPath(_dao.Read(idFolder.Value)) + Path.DirectorySeparatorChar.ToString();
				}
                else
                {
                    return GetUserRootFolder(session.IdUser) + Path.DirectorySeparatorChar.ToString();
                }
			}
			catch (FileSharingException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
			}
		}

		#region Private Methods

		string GetFullPath(Folder folder)
		{
			if (folder.IdFolderRoot.HasValue)
				return GetFullPath(_dao.Read(folder.IdFolderRoot.Value)) + Path.DirectorySeparatorChar.ToString() + folder.Name;
					
			return GetUserRootFolder(folder.IdUser) + Path.DirectorySeparatorChar.ToString() + folder.Name;
		}

		string GetUserRootFolder(long idUser)
		{
			string path = repoPath + Path.DirectorySeparatorChar.ToString() + idUser;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
        }

        List<FolderDto> GetFoldersInFolder(long idUser, long? idFolder)
        {
            try
            {
                var result = _dao.GetFoldersInFolder(idUser, idFolder);
                return Mapper.Map<List<FolderDto>>(result);
            }
            catch (FileSharingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FileSharingException(FileSharingException.ERROR_DOCUMENT_MANAGER_SERVER, e.Message, e);
            }
        }

        #endregion
    }
}

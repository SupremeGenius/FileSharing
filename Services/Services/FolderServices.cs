using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileSharing.Services
{
    public class FolderServices : AbstractServices<FolderDao>
	{
		readonly string repoPath;

		public FolderServices() : base(new FolderDao())
		{
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

                var path = GetFullPath(session.IdUser, folderDom.IdFolderRoot) + Path.DirectorySeparatorChar.ToString() + folderDom.Name;
                Directory.CreateDirectory(path);

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
				throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
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
                throw new FileSharingException(FileSharingException.ERROR_FILESHARING_SERVER, e.Message, e);
            }
        }

		public void Update(string securityToken, FolderDto folder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var folderDom = _dao.ReadFullFolder(folder.Id);
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

                string action = "Update:\r\n" + "-Previous: " + folderDom + "\r\n";
                Mapper.Map(folder, folderDom);
                action += "-Updated: " + folderDom;

                Directory.Move(previousPath, GetFullPath(folderDom));
				Directory.Delete(previousPath);

				_dao.Update(folderDom);
                Audit(session.IdUser, folderDom.Id.ToString(), typeof(Folder).Name, ActionDto.Update, action);
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

		public void Delete(string securityToken, long idFolder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var folderDom = _dao.ReadFullFolder(idFolder);
				if (folderDom == null)
					throw new FileSharingException(FileSharingException.FOLDER_NOT_FOUND,
						 "Folder with id " + idFolder + " does not exist");
				
				if (folderDom.IdUser != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
													   "You do not have permissions to update this folder");
                
                _dao.Delete(folderDom);
                Audit(session.IdUser, folderDom.Id.ToString(), typeof(Folder).Name, ActionDto.Delete, "Folder deletes: " + folderDom);
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

        public FolderDetailsDto GetFolderDetails(string securityToken, long? idFolder)
        {
            try
            {
                var session = CheckSession(securityToken);
                FolderDetailsDto result = new FolderDetailsDto();
                if (idFolder.HasValue)
                {
                    result = Mapper.Map<FolderDetailsDto>(_dao.ReadFullFolder(idFolder.Value));
                }
                else
                {
                    result.Folders = Mapper.Map<List<FolderDto>>(_dao.GetFoldersInRoot(session.IdUser));
                    using (var fileService = new FileServices())
                    {
                        result.Files = fileService.GetFilesInRoot(securityToken);
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

		public string GetFullPath(long idUser, long? idFolder)
		{
			try
			{
				if (idFolder.HasValue)
				{
					var folder = _dao.ReadFullFolder(idFolder.Value);
					if (folder == null)
						throw new FileSharingException(FileSharingException.FOLDER_NOT_FOUND,
							 "Folder with id " + idFolder.Value + " does not exist");
					return GetFullPath(folder) + Path.DirectorySeparatorChar.ToString();
				}
                else
                {
                    return GetUserRootFolder(idUser) + Path.DirectorySeparatorChar.ToString();
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

		#region Private Methods

		string GetFullPath(Folder folder)
		{
			if (folder.IdFolderRoot.HasValue)
				return GetFullPath(folder.FolderRoot) + Path.DirectorySeparatorChar.ToString() + folder.Name;
					
			return GetUserRootFolder(folder.IdUser) + Path.DirectorySeparatorChar.ToString() + folder.Name;
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

using AutoMapper;
using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace FileSharing.Services
{
    public class FileServices : AbstractServices<FileDao>
	{
		public FileServices() : base(new FileDao()) { }

		public long Create(string securityToken, FileDto file)
		{
			try
			{
				var session = CheckSession(securityToken);
                var similarName = _dao.GetByNameAndFolder(file.Filename, file.IdFolder);
                if (similarName != null)
                        throw new FileSharingException(FileSharingException.FILE_ALREADY_EXISTS,
                                                           "You already have a file with this filename");

                file.IdUser = session.IdUser;
                file.CreationDate = file.ModificationDate = DateTime.Now;

                var fileDom = Mapper.Map<File>(file);

                string filePath = GetFilePath(securityToken, fileDom);


                System.IO.File.WriteAllBytes(filePath, file.Content);
				
				fileDom = _dao.Create(fileDom);
                Audit(session.IdUser, fileDom.Id.ToString(), typeof(File).Name, ActionDto.Create, "File uploaded: " + fileDom);
                return fileDom.Id;
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

		public FileDto Read(string securityToken, long idFile)
		{
			try
			{
				var session = CheckSession(securityToken);
				var fileDom = _dao.Read(idFile);
				if (fileDom == null) return null;

				if (fileDom.IdUser != session.IdUser && !fileDom.IsPublic)
				{
					UserGroupDto userGroup = null;
					if (fileDom.IdGroup.HasValue)
					{
						using (var userGroupServices = new UserGroupServices())
						{
							userGroup = userGroupServices.Read(securityToken, session.IdUser, fileDom.IdGroup.Value);
						}
					}
					if (userGroup == null)
						throw new FileSharingException(FileSharingException.UNAUTHORIZED,
														   "You do not have permissions to read this file");
				}

				string filePath = GetFilePath(securityToken, fileDom);

				if (!System.IO.File.Exists(filePath))
					throw new FileSharingException(FileSharingException.FILE_NOT_FOUND,
													   "The file does not exist in the repository");

				var file = Mapper.Map<FileDto>(fileDom);
				file.Content = System.IO.File.ReadAllBytes(filePath);
                file.ContentSize = file.Content.Length/1024;

				return file;
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

		public void Update(string securityToken, FileDto file)
		{
			try
			{
				var session = CheckSession(securityToken);
				var fileDom = _dao.Read(file.Id);
				if (fileDom == null)
					throw new FileSharingException(FileSharingException.FILE_NOT_FOUND,
													   "File with id " + file.Id + " does not exist");
				if (fileDom.IdUser != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
													   "You do not have permissions to update this file");

				string oldFilePath = GetFilePath(securityToken, fileDom);
                var oldFile = fileDom.ToString();
                Mapper.Map(file, fileDom);
                fileDom.ModificationDate = DateTime.Now;
                var newFilePath = GetFilePath(securityToken, fileDom);

                if (oldFilePath != newFilePath)
                {
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);

                    System.IO.File.WriteAllBytes(newFilePath, file.Content);
                }
                
				_dao.Update(fileDom);
                Audit(session.IdUser, fileDom.Id.ToString(), typeof(File).Name, ActionDto.Update,
                       "File updated:\r\n" + "-Previous: " + oldFile + "\r\n" + "-Updated: " + fileDom.ToString());
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

		public long? Delete(string securityToken, long idFile)
		{
			try
			{
				var session = CheckSession(securityToken);
				var fileDom = _dao.Read(idFile);
				if (fileDom == null)
					throw new FileSharingException(FileSharingException.FILE_NOT_FOUND,
													   "File with id " + fileDom.Id + " does not exist");
				if (fileDom.IdUser != session.IdUser)
					throw new FileSharingException(FileSharingException.UNAUTHORIZED,
													   "You do not have permissions to update this file");
				
				_dao.Delete(fileDom);
                Audit(session.IdUser, fileDom.Id.ToString(), typeof(File).Name, ActionDto.Delete, "File deleted: " + fileDom);
                return fileDom.IdFolder;
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

		public List<FileDto> GetFilesInRoot(string securityToken)
		{
			try
			{
				var session = CheckSession(securityToken);
				var result = _dao.GetFilesInRoot(session.IdUser);
				return Mapper.Map<List<FileDto>>(result);
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

        public List<FileDto> GetFilesByGroup(string securityToken, long idGroup)
        {
            try
            {
                var session = CheckSession(securityToken);
                var result = _dao.GetFilesByIdGroup(idGroup);
                return Mapper.Map<List<FileDto>>(result);
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

		#region Private methods

		string GetFilePath(string securityToken, File file)
		{
			using (var folderServices = new FolderServices())
			{
				return folderServices.GetFullPath(securityToken, file.IdFolder) + file.Filename;
			}
		}

		#endregion
	}
}

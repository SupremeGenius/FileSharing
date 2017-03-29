using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using FileStorage.Persistence.Daos;
using FileStorage.Persistence.Models;
using FileStorage.Services.Dtos;
using FileStorage.Services.Exceptions;

namespace FileStorage.Services
{
	public class DocumentServices : AbstractServices<DocumentDao>
	{
		public DocumentServices() : base(new DocumentDao()) { }

		public long Create(string securityToken, DocumentDto document)
		{
			try
			{
				var session = CheckSession(securityToken);

				document.IdUser = session.IdUser;
				var documentDom = Mapper.Map<Document>(document);

				string filePath = GetFilePath(securityToken, documentDom);

				if (File.Exists(filePath))
					throw new FileStorageException(FileStorageException.DOCUMENT_ALREADY_EXISTS,
													   "You already have a document with this filename");

				File.WriteAllBytes(filePath, document.Content);
				
				documentDom = _dao.Create(documentDom);
				//TODO Audit

				return documentDom.Id;
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

		public DocumentDto Read(string securityToken, long idDocument)
		{
			try
			{
				var session = CheckSession(securityToken);
				var documentDom = _dao.Read(idDocument);
				if (documentDom == null) return null;

				if (documentDom.IdUser != session.IdUser && !documentDom.IsPublic)
				{
					UserGroupDto userGroup = null;
					if (documentDom.IdGroup.HasValue)
					{
						using (var userGroupServices = new UserGroupServices())
						{
							userGroup = userGroupServices.Read(securityToken, session.IdUser, documentDom.IdGroup.Value);
						}
					}
					if (userGroup == null)
						throw new FileStorageException(FileStorageException.UNAUTHORIZED,
														   "You do not have permissions to read this document");
				}

				string filePath = GetFilePath(securityToken, documentDom);

				if (!File.Exists(filePath))
					throw new FileStorageException(FileStorageException.FILE_NOT_FOUND,
													   "The file does not exist in the repository");

				var document = Mapper.Map<DocumentDto>(documentDom);
				document.Content = File.ReadAllBytes(filePath);

				return document;
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

		public void Update(string securityToken, DocumentDto document)
		{
			try
			{
				var session = CheckSession(securityToken);
				var documentDom = _dao.Read(document.Id);
				if (documentDom == null)
					throw new FileStorageException(FileStorageException.DOCUMENT_NOT_FOUND,
													   "Document with id " + document.Id + " does not exist");
				if (documentDom.IdUser != session.IdUser)
					throw new FileStorageException(FileStorageException.UNAUTHORIZED,
													   "You do not have permissions to update this document");

				string filePath = GetFilePath(securityToken, documentDom);

				if (File.Exists(filePath))
					File.Delete(filePath);

				File.WriteAllBytes(filePath, document.Content);

				Mapper.Map(document, documentDom);
				_dao.Update(documentDom);
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

		public void Delete(string securityToken, long idDocument)
		{
			try
			{
				var session = CheckSession(securityToken);
				var documentDom = _dao.Read(idDocument);
				if (documentDom == null)
					throw new FileStorageException(FileStorageException.DOCUMENT_NOT_FOUND,
													   "Document with id " + documentDom.Id + " does not exist");
				if (documentDom.IdUser != session.IdUser)
					throw new FileStorageException(FileStorageException.UNAUTHORIZED,
													   "You do not have permissions to update this document");
				
				string filePath = GetFilePath(securityToken, documentDom);

				if (File.Exists(filePath))
					File.Delete(filePath);

				_dao.Delete(documentDom);
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

		public List<DocumentDto> GetDocumentsInFolder(string securityToken, long? idFolder)
		{
			try
			{
				var session = CheckSession(securityToken);
				var result = _dao.GetDocumentsInFolder(session.IdUser, idFolder);
				return Mapper.Map<List<DocumentDto>>(result);
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

		#region Private methods

		string GetFilePath(string securityToken, Document document)
		{
			using (var folderServices = new FolderServices())
			{
				return folderServices.GetFullPath(securityToken, document.IdFolder) + document.Filename;
			}
		}

		#endregion
	}
}

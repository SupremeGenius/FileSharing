using System;
using System.IO;
using AutoMapper;
using DocumentManager.Persistence.Daos;
using DocumentManager.Persistence.Models;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;

namespace DocumentManager.Services
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
					throw new DocumentManagerException(DocumentManagerException.DOCUMENT_ALREADY_EXISTS,
													   "You already have a document with this filename");

				File.WriteAllBytes(filePath, document.Content);
				
				documentDom = _dao.Create(documentDom);
				//TODO Audit

				return documentDom.Id;
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
						throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
														   "You do not have permissions to read this document");
				}

				string filePath = GetFilePath(securityToken, documentDom);

				if (!File.Exists(filePath))
					throw new DocumentManagerException(DocumentManagerException.FILE_NOT_FOUND,
													   "The file does not exist in the repository");

				var document = Mapper.Map<DocumentDto>(documentDom);
				document.Content = File.ReadAllBytes(filePath);

				return document;
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

		public void Update(string securityToken, DocumentDto document)
		{
			try
			{
				var session = CheckSession(securityToken);
				var documentDom = _dao.Read(document.Id);
				if (documentDom == null)
					throw new DocumentManagerException(DocumentManagerException.DOCUMENT_NOT_FOUND,
													   "Document with id " + document.Id + " does not exist");
				if (documentDom.IdUser != session.IdUser)
					throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
													   "You do not have permissions to update this document");

				string filePath = GetFilePath(securityToken, documentDom);

				if (File.Exists(filePath))
					File.Delete(filePath);

				File.WriteAllBytes(filePath, document.Content);

				Mapper.Map(document, documentDom);
				_dao.Update(documentDom);
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

		public void Delete(string securityToken, long idDocument)
		{
			try
			{
				var session = CheckSession(securityToken);
				var documentDom = _dao.Read(idDocument);
				if (documentDom == null)
					throw new DocumentManagerException(DocumentManagerException.DOCUMENT_NOT_FOUND,
													   "Document with id " + documentDom.Id + " does not exist");
				if (documentDom.IdUser != session.IdUser)
					throw new DocumentManagerException(DocumentManagerException.UNAUTHORIZED,
													   "You do not have permissions to update this document");
				
				string filePath = GetFilePath(securityToken, documentDom);

				if (File.Exists(filePath))
					File.Delete(filePath);

				_dao.Delete(documentDom);
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

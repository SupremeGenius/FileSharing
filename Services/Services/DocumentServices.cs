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
		readonly string repoPath;

		public DocumentServices() : base(new DocumentDao()) { }


		public long Create(string securityToken, DocumentDto document)
		{
			var session = CheckSession(securityToken);
			var path = repoPath + "/" + session.IdUser + "/" + document.Filename;
			if (File.Exists(path))
				throw new DocumentManagerException(DocumentManagerException.DOCUMENT_ALREADY_EXISTS,
												   "You already have a document with this filename");
			document.IdUser = session.IdUser;
			File.WriteAllBytes(path, document.Content);

			var documentDom = Mapper.Map<Document>(document);
			documentDom = _dao.Create(documentDom);
			return documentDom.Id;
		}
	}
}

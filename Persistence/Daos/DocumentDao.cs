using System.Collections.Generic;
using System.Linq;
using DocumentManager.Persistence.Models;
using DocumentManager.Persistence.Models.Filters;

namespace DocumentManager.Persistence.Daos
{
	public class DocumentDao : AbstractDao<Document, long>
	{
		public List<Document> QueryVisibleByUser(DocumentFilter filter)
		{
			var query = _dbSet.AsQueryable();
			query = query.Where(d => d.IdUser == filter.IdUser
				|| d.IsPublic == true
				|| (d.IdGroup.HasValue && filter.Groups.Contains(d.IdGroup.Value)));

			if (!string.IsNullOrWhiteSpace(filter.Filename))
			{
				query = query.Where(d => d.Filename.Contains(filter.Filename));
			}
			return query.ToList();
		}

		public List<Document> GetDocumentsInFolder(long idUser, long? idFolder)
		{
			return _dbSet.Where(d => d.IdUser == idUser && d.IdFolder == idFolder).ToList();
		}
	}
}

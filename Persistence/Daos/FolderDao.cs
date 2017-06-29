using System.Linq;
using FileSharing.Persistence.Models;
using System.Collections.Generic;

namespace FileSharing.Persistence.Daos
{
	public class FolderDao : AbstractDao<Folder, long>
	{
		public Folder GetByNameAndFolderRoot(string name, long? idFolderRoot)
		{
			return _dbSet.Where(f => f.Name == name && f.IdFolderRoot == idFolderRoot).FirstOrDefault();
		}

		public List<Folder> GetFoldersInFolder(long idUser, long? idFolder)
		{
			return _dbSet.Where(f => f.IdUser == idUser && f.IdFolderRoot == idFolder).ToList();
		}
	}
}

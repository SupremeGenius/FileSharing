using System.Linq;
using FileStorage.Persistence.Models;

namespace FileStorage.Persistence.Daos
{
	public class FolderDao : AbstractDao<Folder, long>
	{
		public Folder GetByNameAndFolderRoot(string name, long? idFolderRoot)
		{
			return _dbSet.Where(f => f.Name == name && f.IdFolderRoot == idFolderRoot).FirstOrDefault();
		}

		public Folder GetUserRootFolder(long idUser)
		{
			return _dbSet.Where(f => f.IdUser == idUser && !f.IdFolderRoot.HasValue).FirstOrDefault();
		}
	}
}

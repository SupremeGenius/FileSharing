using System.Linq;
using FileSharing.Persistence.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.Persistence.Daos
{
	public class FolderDao : AbstractDao<Folder, long>
	{
		public Folder GetByNameAndFolderRoot(string name, long? idFolderRoot)
		{
			return _dbSet.Where(f => f.Name == name && f.IdFolderRoot == idFolderRoot).FirstOrDefault();
		}

		public List<Folder> GetFoldersInRoot(long idUser)
		{
			return _dbSet.Where(f => f.IdUser == idUser && f.IdFolderRoot == null).ToList();
		}

        public Folder ReadFullFolder(long idFolder)
        {
            return _dbSet
                .Include(f => f.FolderRoot)
                .Include(f => f.Folders)
                .Include(f => f.Documents)
                .Where(f => f.Id == idFolder).FirstOrDefault();
        }
	}
}

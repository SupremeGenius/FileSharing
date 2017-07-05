using FileSharing.Persistence.Models;
using System.Collections.Generic;
using System.Linq;

namespace FileSharing.Persistence.Daos
{
    public class FileDao : AbstractDao<File, long>
	{
        public File GetByNameAndFolder(string filename, long? idFolder)
        {
            return _dbSet.Where(f => f.Filename == filename && f.IdFolder == idFolder).FirstOrDefault();
        }

        public List<File> GetFilesInRoot(long idUser)
		{
			return _dbSet.Where(f => f.IdUser == idUser && f.IdFolder == null).ToList();
		}

        public List<File> GetFilesByIdGroup(long idGroup)
        {
            return _dbSet.Where(f => f.IdGroup == idGroup).ToList();
        }
	}
}

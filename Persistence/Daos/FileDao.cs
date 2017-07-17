using FileSharing.Persistence.Models;
using Microsoft.EntityFrameworkCore;
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

        public List<File> QueryByName(long idUser, string name, int rowQty, int page)
        {
            var query = (from f in _dbSet
                         join ugTemp in _context.UserGroup on f.IdGroup equals ugTemp.IdGroup into ugLeft
                         from ug in ugLeft.DefaultIfEmpty()
                         where f.Filename.Contains(name) && (f.IdUser == idUser || ug.IdUser == idUser || f.IsPublic )
                         select f).Distinct();
            if (rowQty > 0)
            {
                query = query.Skip(page * rowQty).Take(rowQty);
            }
            return query.ToList();
        }
    }
}

using System.Linq;
using FileSharing.Persistence.Models;
using System.Collections.Generic;

namespace FileSharing.Persistence.Daos
{
	public class UserGroupDao : AbstractDao<UserGroup, long[]>
	{
		public UserGroup Read(long idUser, long idGroup)
		{
			return _dbSet.Where(ug => ug.IdUser == idUser && ug.IdGroup == idGroup).FirstOrDefault();
		}

        public List<UserGroup> FindByUser(long idUser)
        {
            return _dbSet.Where(ug => ug.IdUser == idUser).ToList();
        }

        public List<UserGroup> FindByGroup(long idGroup)
        {
            return _dbSet.Where(ug => ug.IdGroup == idGroup).ToList();
        }
    }
}

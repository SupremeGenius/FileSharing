using System.Linq;
using FileStorage.Persistence.Models;

namespace FileStorage.Persistence.Daos
{
	public class UserGroupDao : AbstractDao<UserGroup, long[]>
	{
		public UserGroup Read(long idUser, long idGroup)
		{
			return _dbSet.Where(ug => ug.IdUser == idUser && ug.IdGroup == idGroup).FirstOrDefault();
		}
	}
}

using System.Linq;
using FileSharing.Persistence.Models;

namespace FileSharing.Persistence.Daos
{
	public class UserGroupDao : AbstractDao<UserGroup, long[]>
	{
		public UserGroup Read(long idUser, long idGroup)
		{
			return _dbSet.Where(ug => ug.IdUser == idUser && ug.IdGroup == idGroup).FirstOrDefault();
		}
	}
}

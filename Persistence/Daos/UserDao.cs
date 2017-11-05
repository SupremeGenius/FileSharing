using FileSharing.Persistence.Models;
using System.Linq;

namespace FileSharing.Persistence.Daos
{
    public class UserDao : AbstractDao<User, long>
	{
		public User ReadByLogin(string login)
		{
			return _dbSet.Where(u => u.Login == login).FirstOrDefault();
		}
	}
}

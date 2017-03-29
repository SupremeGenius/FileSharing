using System.Linq;
using FileStorage.Persistence.Models;

namespace FileStorage.Persistence.Daos
{
	public class UserDao : AbstractDao<User, long>
	{
		public User ReadByLogin(string login)
		{
			return _dbSet.Where(u => u.Login == login).FirstOrDefault();
		}
	}
}

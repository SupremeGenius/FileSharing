using System.Linq;
using DocumentManager.Persistence.Models;

namespace DocumentManager.Persistence.Daos
{
	public class UserDao : AbstractDao<User, long>
	{
		public User ReadByLogin(string login)
		{
			return _dbSet.Where(u => u.Login == login).FirstOrDefault();
		}
	}
}

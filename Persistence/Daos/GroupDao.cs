using System.Collections.Generic;
using System.Linq;
using DocumentManager.Persistence.Models;

namespace DocumentManager.Persistence.Daos
{
	public class GroupDao : AbstractDao<Group, long>
	{
		public List<Group> QueryByName(string name)
		{
			return _dbSet.Where(g => g.Name.Contains(name)).ToList();
		}

		public List<Group> GetAdministrableGroups(long idUser)
		{
			return _dbSet.Where(g => g.IdAdmin == idUser).ToList();
		}
	}
}

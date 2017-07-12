using System.Collections.Generic;
using System.Linq;
using FileSharing.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.Persistence.Daos
{
	public class GroupDao : AbstractDao<Group, long>
	{
		public List<Group> QueryByName(string name, int rowQty, int page)
		{
            var query = _dbSet.Where(g => g.Name.Contains(name));
            if (rowQty > 0)
            {
                query = query.Skip(page * rowQty).Take(rowQty);
            }
            return query.ToList();                
		}

		public List<Group> GetAdministrableGroups(long idUser)
		{
			return _dbSet.Where(g => g.IdAdmin == idUser).ToList();
        }

        public Group ReadFullGroup(long idGroup)
        {
            return _dbSet
                .Include(g => g.Files)
                .Include(g => g.Users)
                .Include(g => g.Admin)
                .Where(g => g.Id == idGroup).FirstOrDefault();
        }

        public List<Group> GetByUser(long idUser)
        {
            return (from g in _dbSet
                    join ug in _context.UserGroup on g.Id equals ug.IdGroup
                    where ug.IdUser == idUser
                    select g)
                    .Include(g => g.Files)
                    .Include(g => g.Users)
                    .Include(g => g.Admin).ToList();
        }
    }
}

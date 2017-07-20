using FileSharing.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FileSharing.Persistence.Daos
{
    public class UserGroupDao : AbstractDao<UserGroup, long[]>
	{
		public UserGroup Read(long idUser, long idGroup)
		{
            return _dbSet
                .Where(ug => ug.IdUser == idUser && ug.IdGroup == idGroup)
                .FirstOrDefault();
        }
        
        public UserGroup ReadFull(long idUser, long idGroup)
        {
            return _dbSet
                .Include(ug => ug.Group)
                .Include(ug => ug.User)
                .Where(ug => ug.IdUser == idUser && ug.IdGroup == idGroup)
                .FirstOrDefault();
        }

        public List<UserGroup> GetRequestsByUser(long idUser)
        {
            return _dbSet
                .Include(ug => ug.Group)
                .Include(ug => ug.User)
                .Where(ug => ug.IdUser == idUser && !ug.DateInclusionApproval.HasValue)
                .ToList();
        }
    }
}

using System.Linq;
using FileSharing.Persistence.Models;
using System.Collections.Generic;
using FileSharing.Persistence.Models.Filters;

namespace FileSharing.Persistence.Daos
{
	public class UserGroupDao : AbstractDao<UserGroup, long[]>
	{
		public List<UserGroup> Query(UserGroupFilter filter)
		{
            var query = _dbSet.AsQueryable();

            if (filter.IdUser.HasValue)
                query = query.Where(ug => ug.IdUser == filter.IdUser);
            if (filter.IdGroup.HasValue)
                query = query.Where(ug => ug.IdGroup == filter.IdGroup);
            if (filter.DateInclusionRequestFrom.HasValue)
                query = query.Where(ug => ug.DateInclusionRequest >= filter.DateInclusionRequestFrom);
            if (filter.DateInclusionRequestTo.HasValue)
                query = query.Where(ug => ug.DateInclusionRequest < filter.DateInclusionRequestTo);
            if (filter.DateInclusionApprovalFrom.HasValue)
                query = query.Where(ug => ug.DateInclusionApproval >= filter.DateInclusionApprovalFrom);
            if (filter.DateInclusionApprovalTo.HasValue)
                query = query.Where(ug => ug.DateInclusionApproval < filter.DateInclusionApprovalTo);

            return query.ToList();
        }
    }
}

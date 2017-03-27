using System.Collections.Generic;
using System.Linq;
using DocumentManager.Persistence.Models;
using DocumentManager.Persistence.Models.Filters;

namespace DocumentManager.Persistence.Daos
{
	public class AuditDao : AbstractDao<Audit, long>
	{
		public List<Audit> Query(AuditFilter filter)
		{
			var query = _dbSet.AsQueryable();

			if (filter.IdUser != null)
				query = query.Where(a => a.IdUser == filter.IdUser);
			if (!string.IsNullOrWhiteSpace(filter.Object))
			{
				query = query.Where(a => a.Object == filter.Object);
				if (!string.IsNullOrWhiteSpace(filter.IdObject))
					query = query.Where(a => a.IdObject == filter.IdObject);
			}
			if (filter.DateFrom != null)
				query = query.Where(a => a.Date >= filter.DateFrom);
			if (filter.DateTo != null)
				query = query.Where(a => a.Date < filter.DateTo);

			return query.ToList();
		}
	}
}

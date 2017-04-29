using System;
using System.Linq;
using FileSharing.Persistence.Models;

namespace FileSharing.Persistence.Daos
{
	public class SessionDao : AbstractDao<Session, string>
	{
		public void DeleteExpiredSessions(DateTime date)
		{
			var sessions = _dbSet.Where(s => s.DateLastAccess <= date).ToList();
			foreach (var session in sessions)
			{
				_dbSet.Remove(session);
			}
			_context.SaveChanges();
		}
	}
}

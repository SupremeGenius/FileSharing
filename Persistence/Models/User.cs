using System.Collections.Generic;
namespace DocumentManager.Persistence.Models
{
	public class User : AbstractModel
	{
		public User()
		{
			Audit = new HashSet<Audit>();
			Document = new HashSet<Document>();
			Folder = new HashSet<Folder>();
			Group = new HashSet<Group>();
			Session = new HashSet<Session>();
			UserGroup = new HashSet<UserGroup>();
		}

		public long Id { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public virtual ICollection<Audit> Audit { get; set; }
		public virtual ICollection<Document> Document { get; set; }
		public virtual ICollection<Folder> Folder { get; set; }
		public virtual ICollection<Group> Group { get; set; }
		public virtual ICollection<Session> Session { get; set; }
		public virtual ICollection<UserGroup> UserGroup { get; set; }
	}
}

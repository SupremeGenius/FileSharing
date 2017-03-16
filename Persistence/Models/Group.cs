using System.Collections.Generic;
namespace DocumentManager.Persistence.Models
{
	public class Group : AbstractModel
	{
		public Group()
		{
			Document = new HashSet<Document>();
			UserGroup = new HashSet<UserGroup>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public long IdAdmin { get; set; }

		public virtual ICollection<Document> Document { get; set; }
		public virtual ICollection<UserGroup> UserGroup { get; set; }
		public virtual User IdAdminNavigation { get; set; }
	}
}

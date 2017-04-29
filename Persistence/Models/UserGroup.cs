using System;
namespace FileSharing.Persistence.Models
{
	public class UserGroup : AbstractModel
	{
		public long IdUser { get; set; }
		public long IdGroup { get; set; }
		public DateTime DateInclusionRequest { get; set; }
		public DateTime? DateInclusionApproval { get; set; }

		public virtual Group IdGroupNavigation { get; set; }
		public virtual User IdUserNavigation { get; set; }
	}
}

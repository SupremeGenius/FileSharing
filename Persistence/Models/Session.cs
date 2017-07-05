using System;
namespace FileSharing.Persistence.Models
{
	public class Session : AbstractModel
	{
		public string SecurityToken { get; set; }
		public long IdUser { get; set; }
		public DateTime DateLastAccess { get; set; }

		public virtual User User { get; set; }
	}
}

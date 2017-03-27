using System;
namespace DocumentManager.Persistence.Models
{
	public class Audit : AbstractModel
	{
		public long Id { get; set; }
		public long IdUser { get; set; }
		public DateTime Date { get; set; }
		public string Object { get; set; }
		public string IdObject { get; set; }
		public string Action { get; set; }

		public virtual User IdUserNavigation { get; set; }
	}
}

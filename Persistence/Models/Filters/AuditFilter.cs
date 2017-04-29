using System;
namespace FileSharing.Persistence.Models.Filters
{
	public class AuditFilter
	{
		public long? IdUser { get; set; }
		public string Object { get; set; }
		public string IdObject { get; set; }
        public Action? Action { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
	}
}

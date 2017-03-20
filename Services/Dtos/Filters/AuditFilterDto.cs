using System;
namespace DocumentManager.Services.Filters
{
	public class AuditFilterDto
	{
		public long? IdUser { get; set; }
		public string Object { get; set; }
		public long? IdObject { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
	}
}

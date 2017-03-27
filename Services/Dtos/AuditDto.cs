using System;
namespace DocumentManager.Services.Dtos
{
	public class AuditDto
	{
		public long IdUser { get; set; }
		public DateTime Date { get; set; }
		public string Object { get; set; }
		public string IdObject { get; set; }
		public string Action { get; set; }
	}
}

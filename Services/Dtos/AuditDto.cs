using System;
namespace DocumentManager.Services.Dtos
{
	public class AuditDto
	{
		public long IdUser { get; set; }
		public DateTime Date { get; set; }
		public string Object { get; set; }
		public string IdObject { get; set; }
		public ActionDto? Action { get; set; }
        public string Description { get; set; }
	}

    public enum ActionDto
    {
        Create,
        Update,
        Delete
    }
}

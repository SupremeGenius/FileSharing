using System;
namespace FileSharing.Persistence.Models
{
	public class Audit : AbstractModel
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public DateTime Date { get; set; }
        public string Object { get; set; }
        public string IdObject { get; set; }
        public Action Action { get; set; }
        public string Description { get; set; }
    }

    public enum Action
    {
        Create,
        Update,
        Delete
    }
}

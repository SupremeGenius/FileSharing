namespace DocumentManager.Persistence.Models
{
	public class Document : AbstractModel
	{
		public long Id { get; set; }
		public long IdUser { get; set; }
		public string Filename { get; set; }
		public bool IsPublic { get; set; }
		public long? IdGroup { get; set; }
		public long? IdFolder { get; set; }

		public virtual Folder IdFolderNavigation { get; set; }
		public virtual Group IdGroupNavigation { get; set; }
		public virtual User IdUserNavigation { get; set; }
	}
}

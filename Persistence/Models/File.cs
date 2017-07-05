namespace FileSharing.Persistence.Models
{
	public class File : AbstractModel
	{
		public long Id { get; set; }
		public long IdUser { get; set; }
		public string Filename { get; set; }
		public bool IsPublic { get; set; }
		public long? IdGroup { get; set; }
		public long? IdFolder { get; set; }

		public virtual Folder Folder { get; set; }
		public virtual Group IdGroupNavigation { get; set; }
		public virtual User IdUserNavigation { get; set; }

        public override string ToString()
        {
            string result = "";

            result += "Filename: " + Filename + "\r\n";
            result += "IdUser: " + IdUser + "\r\n";
            result += "IsPublic: " + IsPublic + "\r\n";
            result += "IdGroup: " + (IdGroup.HasValue ? IdGroup.ToString() : "null") + "\r\n";
            result += "IdFolder: " + (IdFolder.HasValue ? IdFolder.ToString() : "null");

            return result;
        }
    }
}

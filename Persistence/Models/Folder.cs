using System.Collections.Generic;

namespace FileSharing.Persistence.Models
{
	public class Folder : AbstractModel
	{
		public Folder()
		{
            Files = new HashSet<File>();
		}

		public long Id { get; set; }
		public long IdUser { get; set; }
		public string Name { get; set; }
		public long? IdFolderRoot { get; set; }

		public virtual ICollection<File> Files { get; set; }
		public virtual Folder FolderRoot { get; set; }
		public virtual ICollection<Folder> Folders { get; set; }
		public virtual User User { get; set; }
        
        public override string ToString()
        {
            string result = "";

            result += "Name: " + Name + "\r\n";
            result += "IdFolderRoot: " + (IdFolderRoot.HasValue ? IdFolderRoot.ToString() : "null");

            return result;
        }
    }
}

using System.Collections.Generic;

namespace FileSharing.Persistence.Models
{
	public class Folder : AbstractModel
	{
		public Folder()
		{
			Document = new HashSet<Document>();
		}

		public long Id { get; set; }
		public long IdUser { get; set; }
		public string Name { get; set; }
		public long? IdFolderRoot { get; set; }

		public virtual ICollection<Document> Document { get; set; }
		public virtual Folder IdFolderRootNavigation { get; set; }
		public virtual ICollection<Folder> InverseIdFolderRootNavigation { get; set; }
		public virtual User IdUserNavigation { get; set; }
        
        public override string ToString()
        {
            string result = "";

            result += "Name: " + Name + "\r\n";
            result += "IdFolderRoot: " + (IdFolderRoot.HasValue ? IdFolderRoot.ToString() : "null");

            return result;
        }
    }
}

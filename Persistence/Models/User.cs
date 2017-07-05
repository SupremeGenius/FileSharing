using System.Collections.Generic;
namespace FileSharing.Persistence.Models
{
    public class User : AbstractModel
	{
		public User()
		{
            Files = new HashSet<File>();
			Folders = new HashSet<Folder>();
			AdministrableGroups = new HashSet<Group>();
			Sessions = new HashSet<Session>();
			Groups = new HashSet<UserGroup>();
		}

		public long Id { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
        
		public virtual ICollection<File> Files { get; set; }
		public virtual ICollection<Folder> Folders { get; set; }
		public virtual ICollection<Group> AdministrableGroups { get; set; }
		public virtual ICollection<Session> Sessions { get; set; }
		public virtual ICollection<UserGroup> Groups { get; set; }

        public override string ToString()
        {
            string result = "";

            result += "Login: " + Login + "\r\n";
            result += "FirstName: " + FirstName + "\r\n";
            result += "LastName: " + LastName;

            return result;
        }
    }
}

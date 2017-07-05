using System;
namespace FileSharing.Persistence.Models
{
	public class UserGroup : AbstractModel
	{
		public long IdUser { get; set; }
		public long IdGroup { get; set; }
		public DateTime DateInclusionRequest { get; set; }
		public DateTime? DateInclusionApproval { get; set; }

		public virtual Group Group { get; set; }
		public virtual User IdUserNavigation { get; set; }

        public override string ToString()
        {
            string result = "";

            result += "IdUser: " + IdUser + "\r\n";
            result += "IdGroup: " + IdGroup + "\r\n";
            result += "DateInclusionRequest: " + DateInclusionRequest + "\r\n";
            result += "DateInclusionApproval: " + (DateInclusionApproval.HasValue ? DateInclusionApproval.Value.ToString() : "null");

            return result;
        }
    }
}

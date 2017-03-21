using System;
namespace DocumentManager.Services.Dtos
{
	public class UserGroupDto
	{
		public long IdUser { get; set; }
		public long IdGroup { get; set; }
		public DateTime DateInclusionRequest { get; set; }
		public DateTime? DateInclusionApproval { get; set; }
	}
}

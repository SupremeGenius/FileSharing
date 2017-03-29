using System;
namespace FileStorage.Services.Dtos
{
	public class SessionDto
	{
		public string SecurityToken { get; set; }
		public long IdUser { get; set; }
		public DateTime DateLastAccess { get; set; }
	}
}

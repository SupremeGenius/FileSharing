using System;
namespace FileStorage.Services.Dtos
{
	public class FolderDto
	{
		public long Id { get; set; }
		public long IdUser { get; set; }
		public string Name { get; set; }
		public long? IdFolderRoot { get; set; }
	}
}

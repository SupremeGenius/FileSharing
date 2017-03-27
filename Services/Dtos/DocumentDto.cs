using System;
namespace DocumentManager.Services.Dtos
{
	public class DocumentDto
	{
		public long Id { get; set; }
		public long IdUser { get; set; }
		public string Filename { get; set; }
		public bool IsPublic { get; set; }
		public long? IdGroup { get; set; }
		public byte[] Content { get; set; }
	}
}

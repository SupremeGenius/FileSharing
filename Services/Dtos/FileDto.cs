using System;

namespace FileSharing.Services.Dtos
{
	public class FileDto
	{
		public long Id { get; set; }
		public long IdUser { get; set; }
		public string Filename { get; set; }
        public string ContentType { get; set; }
        public bool IsPublic { get; set; }
		public long? IdGroup { get; set; }
		public long? IdFolder { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public byte[] Content { get; set; }
        public long ContentSize { get; set; }
	}
}

using System.Collections.Generic;

namespace FileStorage.Persistence.Models.Filters
{
	public class DocumentFilter
	{
		public long IdUser { get; set; }
		public string Filename { get; set; }
		public bool IsPublic { get; set; }
		public List<long> Groups { get; set; }
	}
}

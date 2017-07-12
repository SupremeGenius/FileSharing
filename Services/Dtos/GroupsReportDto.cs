using System.Collections.Generic;

namespace FileSharing.Services.Dtos
{
    public class GroupsReportDto
    {
        public List<GroupDetailsDto> Groups { get; set; }
        public int RowQty { get; set; }
        public int Page { get; set; }
    }
}

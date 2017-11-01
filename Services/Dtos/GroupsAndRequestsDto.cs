using System.Collections.Generic;

namespace FileSharing.Services.Dtos
{
    public class GroupsAndRequestsDto
    {
        public List<GroupDetailsDto> Groups { get; set; }
        public List<UserGroupDetailsDto> Requests { get; set; }
    }
}

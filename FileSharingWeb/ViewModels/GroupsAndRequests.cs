using FileSharing.Services.Dtos;
using System.Collections.Generic;

namespace FileSharingWeb.ViewModels
{
    public class GroupsAndRequests
    {
        public List<GroupDetailsDto> Groups { get; set; }
        public List<UserGroupDetailsDto> Requests { get; set; }
    }
}

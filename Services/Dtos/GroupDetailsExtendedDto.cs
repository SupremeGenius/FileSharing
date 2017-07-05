using System.Collections.Generic;

namespace FileSharing.Services.Dtos
{
    public class GroupDetailsExtendedDto : GroupDetailsDto
    {
        public List<FileDto> Files { get; set; }
        public List<UserDto> Members { get; set; }
    }
}

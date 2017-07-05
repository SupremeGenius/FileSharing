using System.Collections.Generic;

namespace FileSharing.Services.Dtos
{
    public class GroupDetailsExtendedDto : GroupDetailsDto
    {
        public List<DocumentDto> Documents { get; set; }
        public List<UserDto> Members { get; set; }
    }
}

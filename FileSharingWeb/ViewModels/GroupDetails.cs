using System.Collections.Generic;

namespace FileSharingWeb.ViewModels
{
    public class GroupDetails : Group
    {
        public List<File> Files { get; set; }
        public List<User> Members { get; set; }
    }
}

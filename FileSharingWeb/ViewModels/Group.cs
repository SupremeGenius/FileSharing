namespace FileSharingWeb.ViewModels
{
    public class Group
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long NumOfMembers { get; set; }
        public bool IsAdministrable { get; set; }
    }
}

namespace FileSharingWeb.ViewModels
{
    public class File
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
    }

    public enum FileType
    {
        Folder = 0,
        Document = 1,
    }
}

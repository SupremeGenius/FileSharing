using FileSharing.Services;

namespace FileSharingWeb
{
	public class ServicesFactory
	{
		public ServicesFactory()
		{
			User = new UserServices();
			Session = new SessionServices();
			Group = new GroupServices();
			UserGroup = new UserGroupServices();
			Folder = new FolderServices();
            File = new FileServices();
		}

		public UserServices User { get; set; }
		public SessionServices Session { get; set; }
		public GroupServices Group { get; set; }
		public UserGroupServices UserGroup { get; set; }
		public FolderServices Folder { get; set; }
		public FileServices File { get; set; }
	}
}

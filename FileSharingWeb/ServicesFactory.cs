using FileSharing.Services;

namespace FileSharingWeb
{
	public static class ServicesFactory
	{
		public static UserServices User
        {
            get
            {
                return new UserServices();
            }
        }
		public static SessionServices Session
        {
            get
            {
                return new SessionServices();
            }
        }
        public static GroupServices Group
        {
            get
            {
                return new GroupServices();
            }
        }
        public static UserGroupServices UserGroup
        {
            get
            {
                return new UserGroupServices();
            }
        }
        public static FolderServices Folder
        {
            get
            {
                return new FolderServices();
            }
        }
        public static FileServices File
        {
            get
            {
                return new FileServices();
            }
        }
    }
}

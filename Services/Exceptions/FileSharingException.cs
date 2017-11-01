using System;

namespace FileSharing.Services.Exceptions
{
	public class FileSharingException : Exception
	{
		public string Code { get; protected set; }

		#region Const strings

		public const string ERROR_FILESHARING_SERVER = "ERROR_FILESHARING_SERVER";
		public const string NULL_VALUE = "NULL_VALUE";
		public const string UNAUTHORIZED = "UNAUTHORIZED";

        #region File

        public const string FILE_ALREADY_EXISTS = "FILE_ALREADY_EXISTS";
		public const string FILE_NOT_FOUND = "FILE_NOT_FOUND";

		#endregion

		#region Folder

		public const string FOLDER_ALREADY_EXISTS = "FOLDER_ALREADY_EXISTS";
		public const string FOLDER_NOT_FOUND = "FOLDER_NOT_FOUND";

		#endregion

		#region Goup

		public const string GROUP_NAME_ALREADY_IN_USE = "GROUP_NAME_ALREADY_IN_USE";
		public const string GROUP_NOT_FOUND = "GROUP_NOT_FOUND";
        public const string CANNOT_DELETE_GROUP_WITH_MEMBERS = "CANNOT_DELETE_GROUP_WITH_MEMBERS";

		#endregion

		#region Session

		public const string SESSION_NOT_FOUND = "SESSION_NOT_FOUND";

		#endregion

		#region User

		public const string LOGIN_ALREADY_IN_USE = "LOGIN_ALREADY_IN_USE";
		public const string USER_NOT_FOUND = "USER_NOT_FOUND";
		public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
		public const string USER_CANNOT_BE_REMOVED = "USER_CANNOT_BE_REMOVED";
        public const string USERNAME_FIELD_LENGTH = "USERNAME_FIELD_LENGTH";
        public const string PASSWORD_FIELD_LENGTH = "PASSWORD_FIELD_LENGTH";
        public const string PASSWORD_NOT_MATCHING = "PASSWORD_NOT_MATCHING";

        #endregion

        #region UserGroup

        public const string USER_GROUP_ALREADY_REQUESTED = "USER_GROUP_ALREADY_REQUESTED";
        public const string USER_GROUP_ALREADY_MEMBER = "USER_GROUP_ALREADY_MEMBER";

		#endregion

		#endregion

		public FileSharingException(string code, string message)
			: base(message)
		{
			Code = code;
		}

		public FileSharingException(string code, string message, Exception inner)
			: base(message, inner)
		{
			Code = code;
		}
	}
}

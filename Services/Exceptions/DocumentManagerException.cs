using System;

namespace DocumentManager.Services.Exceptions
{
	public class DocumentManagerException : Exception
	{
		public string Code { get; protected set; }

		#region Const strings

		public const string ERROR_DOCUMENT_MANAGER_SERVER = "ERROR_DOCUMENT_MANAGER_SERVER";
		public const string NULL_VALUE = "NULL_VALUE";
		public const string UNAUTHORIZED = "UNAUTHORIZED";

		#region User

		public const string LOGIN_ALREADY_IN_USE = "LOGIN_ALREADY_IN_USE";
		public const string USER_NOT_FOUND = "USER_NOT_FOUND";
		public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";

		#endregion

		#region Session

		public const string SESSION_NOT_FOUND = "SESSION_NOT_FOUND";

		#endregion

		#region Goup

		public const string GROUP_NAME_ALREADY_IN_USE = "GROUP_NAME_ALREADY_IN_USE";
		public const string GROUP_NOT_FOUND = "GROUP_NOT_FOUND";

		#endregion

		#endregion

		public DocumentManagerException(string code, string message)
			: base(message)
		{
			Code = code;
		}

		public DocumentManagerException(string code, string message, Exception inner)
			: base(message, inner)
		{
			Code = code;
		}
	}
}

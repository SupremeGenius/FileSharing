using System;

namespace DocumentManager.Services.Exceptions
{
	public class DocumentManagerException : Exception
	{
		public string Code { get; protected set; }

		#region Const strings

		public const string ERROR_DOCUMENT_MANAGER_SERVER = "ERROR_DOCUMENT_MANAGER_SERVER";
		public const string NULL_VALUE = "NULL_VALUE";

		#region User

		public const string LOGIN_ALREADY_IN_USE = "LOGIN_ALREADY_IN_USE";
		public const string USER_NOT_FOUND = "USER_NOT_FOUND";
		public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";

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

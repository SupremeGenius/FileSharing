﻿namespace FileSharing.Services.Dtos
{
	public class UserDto
	{
		public long Id { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}

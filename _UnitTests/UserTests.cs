using DocumentManager.Services.Dtos;
using DocumentManager.Services;
using Xunit;

namespace _UnitTests
{
    public class UserTests
    {
		readonly UserServices _userServices;
		static UserDto user;
		static string securityToken;
		static string securityToken2;

		public UserTests()
		{
			_userServices = new UserServices();
		}

        [Fact]
        public void RegisterUser()
        {
            user = new UserDto
            {
                Login = "Login",
                Password = "Password",
                FirstName = "FirstName",
                LastName = "LastName",
            };
            user.Id = _userServices.Register(user);

			Assert.True(user.Id > 0);
		}

		[Fact]
		public void Login()
		{
			securityToken = _userServices.Login(user.Login, user.Password);
			Assert.False(string.IsNullOrWhiteSpace(securityToken));
		}

		[Fact]
		public void ChangePassword()
		{
			string newPassword = "NewPassword";
			_userServices.ChangePassword(securityToken, user.Password, newPassword);
			user.Password = newPassword;
			securityToken2 = _userServices.Login(user.Login, user.Password);
			Assert.False(string.IsNullOrEmpty(securityToken2));
		}

		[Fact]
		public void Logout()
		{
			_userServices.Logout(securityToken2);
		}

		[Fact]
		public void ReadUser()
		{
			var userTest = _userServices.Read(securityToken);
			Assert.NotNull(userTest);
			Assert.True(user.Login == userTest.Login);
		}

		[Fact]
		public void UpdateUser()
		{
			UserDto userTest;
			user.FirstName = "Name Update";
			_userServices.Update(securityToken, user);
			userTest = _userServices.Read(securityToken);
			Assert.True(userTest.FirstName == user.FirstName);
		}

		[Fact]
		public void DeleteUser()
		{
			_userServices.Delete(securityToken);
			var userTest = _userServices.Read(securityToken);
			Assert.Null(userTest);
		}
    }
}

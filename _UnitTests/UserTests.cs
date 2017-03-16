using DocumentManager.Services.Dtos;
using DocumentManager.Services;
using Xunit;

namespace _UnitTests
{
    public class UserTests
    {
		readonly UserServices _userServices;
		static UserDto user;

		public UserTests()
		{
			_userServices = new UserServices();

			user = new UserDto
			{
				Login = "Login",
				Password = "Password",
				FirstName = "FirstName",
				LastName = "LastName",
			};
		}

        [Fact]
        public void CreateUser()
		{
			user.Id = _userServices.Create(user);

			Assert.True(user.Id > 0);
		}

		[Fact]
		public void ReadUser()
		{
			var userTest = _userServices.Read(user.Id);
			Assert.NotNull(userTest);
			Assert.True(user.Login == userTest.Login);
		}

        [Fact]
		public void CheckUserPassword()
		{
			var valid = _userServices.CheckUserPassword(user.Login, user.Password);
			Assert.True(valid > 0);
		}

		[Fact]
		public void UpdateUser()
		{
			UserDto userTest;
			user.FirstName = "Name Update";
			_userServices.Update(user);
			userTest = _userServices.Read(user.Id);
			Assert.True(userTest.FirstName == user.FirstName);
		}

		[Fact]
		public void DeleteUser()
		{
			_userServices.Delete(user.Id);
			var userTest = _userServices.Read(user.Id);
			Assert.Null(userTest);
		}
    }
}

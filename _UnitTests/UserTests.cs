using DocumentManager.Services.Dtos;
using DocumentManager.Services.Services;
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
    }
}

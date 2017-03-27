using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocumentManager.Services;
using DocumentManager.Services.Dtos;
using DocumentManager.Services.Exceptions;

namespace _UnitTest
{
    [TestClass]
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

        [TestMethod, Priority(1)]
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

            Assert.IsTrue(user.Id > 0);
        }

        [TestMethod, Priority(2)]
        public void Login()
        {
            securityToken = _userServices.Login(user.Login, user.Password);
            Assert.IsFalse(string.IsNullOrWhiteSpace(securityToken));
        }

        [TestMethod, Priority(3)]
        public void ChangePassword()
        {
            string newPassword = "NewPassword";
            _userServices.ChangePassword(securityToken, user.Password, newPassword);
            user.Password = newPassword;
            securityToken2 = _userServices.Login(user.Login, user.Password);
            Assert.IsFalse(string.IsNullOrEmpty(securityToken2));
        }

        [TestMethod, Priority(4)]
        public void Logout()
        {
            _userServices.Logout(securityToken2);
        }

        [TestMethod, Priority(4)]
        public void ReadUser()
        {
            var userTest = _userServices.Read(securityToken);
            Assert.IsNotNull(userTest);
            Assert.IsTrue(user.Login == userTest.Login);
        }

        [TestMethod, Priority(4)]
        public void UpdateUser()
        {
            UserDto userTest;
            user.FirstName = "Name Update";
            _userServices.Update(securityToken, user);
            userTest = _userServices.Read(securityToken);
            Assert.IsTrue(userTest.FirstName == user.FirstName);
        }

        [TestMethod, Priority(5)]
        [ExpectedException(typeof(DocumentManagerException))]
        public void DeleteUser()
        {
            _userServices.Delete(securityToken);
            var userTest = _userServices.Read(securityToken);
        }
    }
}

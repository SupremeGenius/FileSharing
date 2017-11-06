using FileSharing.Persistence.Context;
using FileSharing.Services;
using FileSharing.Services.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IntegrationTests
{
    [TestClass]
    public class It1Tests : IDisposable
    {
        private string username = "IntegrationUser";
        private string password = "IntegrationPassword";
        
        public It1Tests()
        {
            new ContextFactory().CreateDbContext(null);
        }

        [TestMethod]
        public void Iteration1()
        {
            RegisterUser();
            Login();
        }

        private void RegisterUser()
        {
            using (var userService = new UserServices())
            {
                var securityToken = userService.Register(new UserRegistrationDto
                {
                    FirstName = "Integration",
                    LastName = "Test",
                    Username = username,
                    Password = password,
                    ConfirmPassword = password
                });

                Assert.IsFalse(string.IsNullOrWhiteSpace(securityToken));

                var user = userService.Read(securityToken);
                Assert.IsNotNull(user);
                Assert.IsTrue(user.Id > 0);
            }
        }

        private void Login()
        {
            using (var userService = new UserServices())
            {
                var securityToken = userService.Login(new UserLoginDto
                {
                    Username = username,
                    Password = password
                });

                Assert.IsFalse(string.IsNullOrWhiteSpace(securityToken));

                var user = userService.Read(securityToken);
                Assert.IsNotNull(user);
                Assert.IsTrue(user.Id > 0);
            }
        }
        
        public void Dispose()
        {
            new ContextFactory().CreateDbContext(null).Database.EnsureDeleted();
        }
    }
}

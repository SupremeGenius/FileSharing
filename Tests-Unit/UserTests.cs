using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests_Unit
{
    [TestClass]
    public class UserTests
    {
        private UserDao _dao
        {
            get
            {
                return new UserDao();
            }
        }

        private User userData = new User
        {
            Login = "UserTest",
            Password = "Testing0.",
            FirstName = "User",
            LastName = "Test"
        };

        [TestMethod]
        public void CreateUser()
        {
            using (_dao)
            {
                userData = _dao.Create(userData);
                Assert.IsTrue(userData.Id > 0);
            }
        }

        [TestMethod]
        public void ReadUser()
        {
            CreateUser();
            using (_dao)
            {
                var user = _dao.Read(userData.Id);
                Assert.IsNotNull(userData);
                Assert.AreEqual(userData.Login, user.Login);
            }
        }

        [TestMethod]
        public void ReadUserByLogin()
        {
            CreateUser();
            using (_dao)
            {
                var user = _dao.ReadByLogin(userData.Login);
                Assert.IsNotNull(userData);
                Assert.AreEqual(userData.Login, user.Login);
            }
        }
    }
}

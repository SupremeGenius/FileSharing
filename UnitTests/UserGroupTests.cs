using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class UserGroupTests
    {
        private UserGroup userGroupData = new UserGroup
        {
            IdGroup = 1,
            IdUser = 1,
            DateInclusionRequest = DateTime.Now
        };

        private UserGroupDao _dao = new UserGroupDao();

        [TestMethod]
        public void CreateUserGroup()
        {
            var userGroup = _dao.Create(userGroupData);
            Assert.IsNotNull(userGroup);
        }

        [TestMethod]
        public void UpdateUserGroup()
        {
            try
            {
                CreateUserGroup();
            }
            catch (Exception) { }
            userGroupData.DateInclusionApproval = DateTime.Now;
            _dao.Update(userGroupData);
            var userGroup = _dao.Read(userGroupData.IdUser, userGroupData.IdGroup);
            Assert.AreEqual(userGroupData.DateInclusionApproval, userGroup.DateInclusionApproval);
        }

        [TestMethod]
        public void DeleteUserGroup()
        {
            try
            {
                CreateUserGroup();
            }
            catch (Exception) { }
            _dao.Delete(userGroupData);
            var userGroup = _dao.Read(userGroupData.IdUser, userGroupData.IdGroup);
            Assert.IsNull(userGroup);
        }
    }
}

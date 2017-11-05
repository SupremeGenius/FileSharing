using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GroupTests
    {
        private Group groupData = new Group
        {
            IdAdmin = 1,
            Name = "GroupTest"
        };

        private GroupDao _dao = new GroupDao();

        [TestMethod]
        public void CreateGroup()
        {
            groupData = _dao.Create(groupData);
            Assert.IsTrue(groupData.Id > 0);
        }

        [TestMethod]
        public void ReadGroup()
        {
            CreateGroup();
            var group = _dao.Read(groupData.Id);
            Assert.IsNotNull(group);
            Assert.AreEqual(groupData.Id, group.Id);
        }

        [TestMethod]
        public void ReadGroupByName()
        {
            CreateGroup();
            var groups = _dao.QueryByName(groupData.Name, 0, 0);
            Assert.IsNotNull(groups);
            Assert.IsTrue(groups.Count > 0);
            Assert.AreEqual(groupData.Name, groups[0].Name);
        }
    }
}

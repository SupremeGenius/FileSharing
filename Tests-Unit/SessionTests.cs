using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class SessionTests
    {
        private Session sessionData = new Session
        {
            SecurityToken = Guid.NewGuid().ToString(),
            DateLastAccess = DateTime.Now,
            IdUser = 1
        };

        private SessionDao _dao = new SessionDao();

        [TestMethod]
        public void CreateSession()
        {
            var session =_dao.Create(sessionData);
            Assert.IsNotNull(session);
            Assert.AreEqual(sessionData.SecurityToken, session.SecurityToken);
        }

        [TestMethod]
        public void ReadSession()
        {
            CreateSession();
            var session = _dao.Read(sessionData.SecurityToken);
            Assert.IsNotNull(session);
            Assert.AreEqual(sessionData.SecurityToken, session.SecurityToken);
        }

        [TestMethod]
        public void UpdateSession()
        {
            CreateSession();
            sessionData.DateLastAccess = DateTime.Now;
            _dao.Update(sessionData);
            var session = _dao.Read(sessionData.SecurityToken);
            Assert.AreEqual(sessionData.DateLastAccess, session.DateLastAccess);
        }

        [TestMethod]
        public void DeleteSession()
        {
            CreateSession();
            _dao.Delete(sessionData);
            var session = _dao.Read(sessionData.SecurityToken);
            Assert.IsNull(session);
        }
    }
}

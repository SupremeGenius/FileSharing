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

        private SessionDao _dao
        {
            get
            {
                return new SessionDao();
            }
        }

        [TestMethod]
        public void CreateSession()
        {
            using (_dao)
            {
                var session =_dao.Create(sessionData);
                Assert.IsNotNull(session);
                Assert.AreEqual(sessionData.SecurityToken, session.SecurityToken);
            }
        }

        [TestMethod]
        public void ReadSession()
        {
            CreateSession();
            using (_dao)
            {
                var session = _dao.Read(sessionData.SecurityToken);
                Assert.IsNotNull(session);
                Assert.AreEqual(sessionData.SecurityToken, session.SecurityToken);
            }
        }

        [TestMethod]
        public void UpdateSession()
        {
            CreateSession();
            using (_dao)
            {
                sessionData.DateLastAccess = DateTime.Now;
                _dao.Update(sessionData);
                var session = _dao.Read(sessionData.SecurityToken);
                Assert.AreEqual(sessionData.DateLastAccess, session.DateLastAccess);
            }
        }

        [TestMethod]
        public void DeleteSession()
        {
            CreateSession();
            using (_dao)
            {
                _dao.Delete(sessionData);
                var session = _dao.Read(sessionData.SecurityToken);
                Assert.IsNull(session);
            }
        }
    }
}

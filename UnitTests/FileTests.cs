using FileSharing.Persistence.Daos;
using FileSharing.Persistence.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class FileTests
    {
        private File fileData = new File
        {
            IdUser = 1,
            Filename = "fileTest.txt",
            ContentType = "text/plain",
            CreationDate = DateTime.Now
        };

        private FileDao _dao = new FileDao();

        [TestMethod]
        public void CreateFile()
        {
            fileData = _dao.Create(fileData);
            Assert.IsTrue(fileData.Id > 0);
        }

        [TestMethod]
        public void ReadFile()
        {
            CreateFile();
            var file = _dao.Read(fileData.Id);
            Assert.IsNotNull(file);
            Assert.AreEqual(fileData.Id, file.Id);
        }

        [TestMethod]
        public void ShareFile()
        {
            CreateFile();
            fileData.IsPublic = true;
            _dao.Update(fileData);
            var file = _dao.Read(fileData.Id);
            Assert.IsTrue(file.IsPublic);
            fileData.IdGroup = 3;
            _dao.Update(fileData);
            file = _dao.Read(fileData.Id);
            Assert.AreEqual(fileData.IdGroup, file.IdGroup);
        }

        [TestMethod]
        public void DeleteFile()
        {
            CreateFile();
            _dao.Delete(fileData);
            var file = _dao.Read(fileData.Id);
            Assert.IsNull(file);
        }
    }
}

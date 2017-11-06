using FileSharing.Persistence.Context;
using FileSharing.Services;
using FileSharing.Services.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IntegrationTests
{
    [TestClass]
    public class It3Tests : IDisposable
    {
        private string username = "IntegrationUser";
        private string password = "IntegrationPassword";
        private string securityToken;
        private long idFile;
        private long idGroup;

        public It3Tests()
        {
            using (var userService = new UserServices())
            using (var groupService = new GroupServices())
            {
                securityToken = userService.Register(new UserRegistrationDto
                {
                    FirstName = "Integration",
                    LastName = "1",
                    Username = username,
                    Password = password,
                    ConfirmPassword = password
                });
                var group = groupService.Create(securityToken, "GroupTest");
                idGroup = group;
            }
        }

        [TestMethod]
        public void Iteration3()
        {
            UploadFile();
        }

        private void UploadFile()
        {
            using (var fileService = new FileServices())
            {
                idFile = fileService.Create(securityToken, new FileDto
                {
                    ContentSize = "0",
                    ContentType = "image/jpeg",
                    Filename = "test.jpg"
                }, new byte[0]);
                Assert.IsTrue(idFile > 0);
            }
        }

        private void ModifyFile()
        {
            using (var fileService = new FileServices())
            {
                fileService.Update(securityToken, new FileDto
                {
                    ContentSize = "0",
                    ContentType = "image/jpeg",
                    Filename = "test_renamed.jpg",
                    IdGroup = idGroup,
                    Id = idFile,
                    IsPublic = true
                });
                var file = fileService.Read(securityToken, idFile);
                Assert.IsNotNull(file);
                Assert.IsTrue(file.IsPublic);
                Assert.AreEqual(idGroup, file.IdGroup);
            }
        }

        private void SearchFiles()
        {
            using (var fileService = new FileServices())
            {
                var files = fileService.QueryByName(securityToken, "test", 0, 0);
                Assert.IsNotNull(files);
                Assert.IsTrue(files.Count > 0);
                Assert.AreEqual(idFile, files[0].Id);
            }
        }

        public void Dispose()
        {
            new ContextFactory().CreateDbContext(null).Database.EnsureDeleted();
        }
    }
}

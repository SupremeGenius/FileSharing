using FileSharing.Persistence.Context;
using FileSharing.Services;
using FileSharing.Services.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IntegrationTests
{
    [TestClass]
    public class It2Tests : IDisposable
    {
        private string username = "IntegrationUser";
        private string password = "IntegrationPassword";
        private string securityToken;
        private long idUser;
        private long idGroup;
        
        public It2Tests()
        {
            using (var userService = new UserServices())
            {
                securityToken = userService.Register(new UserRegistrationDto
                {
                    FirstName = "Integration",
                    LastName = "1",
                    Username = username,
                    Password = password,
                    ConfirmPassword = password
                });

                var st = userService.Register(new UserRegistrationDto
                {
                    FirstName = "Integration",
                    LastName = "2",
                    Username = username + "2",
                    Password = password,
                    ConfirmPassword = password
                });
                idUser = userService.Read(st).Id;
            }
        }

        [TestMethod]
        public void Iteration2()
        {
            CreateGroup();
            CreateUnionRequest();
            AcceptUnionRequest();
            RejectUnionRequest();
        }

        private void CreateGroup()
        {
            using (var groupService = new GroupServices())
            {
                var group = groupService.Create(securityToken, "GroupTest");
                idGroup = group;
                Assert.IsTrue(idGroup > 0);
            }
        }

        private void CreateUnionRequest()
        {
            using (var userService = new UserServices())
            using (var userGroupService = new UserGroupServices())
            {
                userGroupService.Create(securityToken, new UserGroupDto
                {
                    DateInclusionRequest = DateTime.Now,
                    IdGroup = idGroup,
                    IdUser = idUser
                });
                var userGroup = userGroupService.Read(securityToken, idUser, idGroup);
                Assert.IsNotNull(userGroup);
            }
        }

        private void AcceptUnionRequest()
        {
            using (var userService = new UserServices())
            using (var userGroupService = new UserGroupServices())
            {
                userGroupService.Accept(securityToken, idUser, idGroup);
                var userGroup = userGroupService.Read(securityToken, idUser, idGroup);
                Assert.IsNotNull(userGroup?.DateInclusionApproval);
            }
        }

        private void RejectUnionRequest()
        {
            using (var userService = new UserServices())
            using (var userGroupService = new UserGroupServices())
            {
                userGroupService.Reject(securityToken, idUser, idGroup);
                var userGroup = userGroupService.Read(securityToken, idUser, idGroup);
                Assert.IsNull(userGroup);
            }
        }

        public void Dispose()
        {
            new ContextFactory().CreateDbContext(null).Database.EnsureDeleted();
        }
    }
}

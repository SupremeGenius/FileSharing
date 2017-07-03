using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using FileSharingWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FileSharingWeb.Controllers
{
    public class GroupsController : BaseController
    {
        readonly ILogger _logger;
        readonly IStringLocalizer _localizer;

        public GroupsController(ILoggerFactory loggerFactory, IStringLocalizerFactory factory)
        {
            _logger = loggerFactory.CreateLogger<PublicController>();
            var type = typeof(Resources);
            _localizer = factory.Create(type);
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Group> result = new List<Group>();
            try
            {
                var groups = Services.UserGroup.GetGroupsOfUser(SecurityToken);
                var user = Services.User.Read(SecurityToken);
                foreach(var group in groups)
                {
                    result.Add(new Group
                    {
                        Id = group.Id,
                        Name = group.Name,
                        NumOfMembers = Services.UserGroup.NumOfMembersOfAGroup(SecurityToken, group.Id),
                        IsAdministrable = group.IdAdmin == user.Id
                    });
                }
            }
            catch (Exception e)
            {
                //TODO Mostrar error
                _logger.LogError(2, e.Message);
            }
            return View(result);
        }

        [HttpPost]
        public IActionResult CreateGroup(string groupName)
        {
            try
            {
                Services.Group.Create(SecurityToken, groupName);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(2, e.Message);
            }
            return RedirectToAction("Index", "Groups");
        }

        [HttpGet]
        public IActionResult Details(long? id)
        {
            GroupDetails result;
            if (!id.HasValue) return View();
            try
            {
                var group = Services.Group.Read(SecurityToken, id.Value);
                var user = Services.User.Read(SecurityToken);
                result = new GroupDetails
                {
                    Id = id.Value,
                    Name = group.Name,
                    IsAdministrable = group.IdAdmin == user.Id,
                    Files = new List<File>(),
                    Members = new List<User>
                    {
                         new User
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Username = user.Login + " (" + _localizer["YOU"] + ")"
                        }
                    }
                };

                var files = Services.Document.GetDocumentsByGroup(SecurityToken, id.Value);
                foreach (var file in files)
                {
                    result.Files.Add(new File
                    {
                        Id = file.Id,
                        Name = file.Filename,
                        Type = FileType.Document
                    });
                }

                var members = Services.UserGroup.GetUsersOfGroup(SecurityToken, id.Value);
                foreach (var member in members)
                {
                    if (member.Id == user.Id) continue;
                    result.Members.Add(new User
                    {
                        Id = member.Id,
                        FullName = member.FullName,
                        Username = member.Login
                    });
                }
            }
            catch (FileSharingException e)
            {
                _logger.LogError(2, e.Message);
                return View();
            }
            return View(result);
        }

        [HttpGet]
        public IActionResult DeleteGroup(long id)
        {
            try
            {
                Services.Group.Delete(SecurityToken, id);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(2, e.Message);
                return View();
            }
            return RedirectToAction("Index", "Groups");
        }
    }
}

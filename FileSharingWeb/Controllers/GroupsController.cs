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
                var groups = Services.Group.GetGroupsOfUser(SecurityToken);
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
            Group result;
            if (!id.HasValue) return View();
            try
            {
                var group = Services.Group.Read(SecurityToken, id.Value);
                var user = Services.User.Read(SecurityToken);
                result = new Group {
                    Id = id.Value,
                    Name = group.Name,
                    IsAdministrable = group.IdAdmin == user.Id
                };
            }
            catch (FileSharingException e)
            {
                _logger.LogError(2, e.Message);
                return View();
            }
            return View(result);
        }
    }
}

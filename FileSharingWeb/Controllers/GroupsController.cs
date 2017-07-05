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
            List<GroupDetailsDto> result = new List<GroupDetailsDto>();
            try
            {
                result = Services.Group.GetGroupsOfUser(SecurityToken);
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
            GroupDetailsExtendedDto result;
            if (!id.HasValue) return View();
            try
            {
                result = Services.Group.GetGroupDetails(SecurityToken, id.Value);
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

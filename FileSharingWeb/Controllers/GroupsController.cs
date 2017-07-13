using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FileSharingWeb.Controllers
{
    public class GroupsController : BaseController
    {
        readonly ILogger<GroupsController> _logger;
        readonly IStringLocalizer _localizer;

        public GroupsController(ILogger<GroupsController> logger, IStringLocalizerFactory factory)
        {
            _logger = logger;
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
                _logger.LogError(e.Message);
                //TODO Mostrar error
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
                _logger.LogError(e.Message);
            }
            return Json(Url.Action("Index", "Groups"));
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
                _logger.LogError(e.Message);
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
                _logger.LogError(e.Message);
                return View();
            }
            return RedirectToAction("Index", "Groups");
        }

        [HttpGet]
        public IActionResult GetGroups()
        {
            List<GroupDetailsDto> result = new List<GroupDetailsDto>();
            try
            {
                result = Services.Group.GetGroupsOfUser(SecurityToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return Json(result);
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Query(string name, int rowQty, int page)
        {
            List<GroupDetailsDto> result = new List<GroupDetailsDto>();
            try
            {
                result = Services.Group.QueryByName(SecurityToken, name, rowQty, page);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return Json(result);
        }
    }
}

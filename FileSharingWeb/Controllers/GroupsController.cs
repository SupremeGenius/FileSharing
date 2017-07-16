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
        public IActionResult Index(string ErrorMessage)
        {
            List<GroupDetailsDto> result = new List<GroupDetailsDto>();
            ViewBag.ErrorMessage = ErrorMessage;
            try
            {
                result = Services.Group.GetGroupsOfUser(SecurityToken);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                ViewBag.ErrorMessage = _localizer[e.Code];
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
        public IActionResult Details(long? id, string ErrorMessage)
        {
            GroupDetailsExtendedDto result;
            ViewBag.ErrorMessage = ErrorMessage;
            if (!id.HasValue) return View();
            try
            {
                result = Services.Group.GetGroupDetails(SecurityToken, id.Value);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Index", "Groups", new { ErrorMessage = _localizer[e.Code] });
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
                return RedirectToAction("Details", "Groups", new { id = id, ErrorMessage = _localizer[e.Code] });
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
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
            }
            return Json(result);
        }

        [HttpGet]
        public IActionResult Query(string name, int rowQty, int page)
        {
            List<GroupDetailsDto> result = new List<GroupDetailsDto>();
            try
            {
                result = Services.Group.QueryByName(SecurityToken, name, rowQty, page);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
            }
            return Json(result);
        }

        [HttpPost]
        public IActionResult RequestGroupUnion(long id)
        {
            try
            {
                var user = Services.User.Read(SecurityToken);
                var result = new UserGroupDto
                {
                    IdGroup = id,
                    IdUser = user.Id,
                    DateInclusionRequest = DateTime.Now
                };
                Services.UserGroup.Create(SecurityToken, result);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(_localizer[e.Code].Value);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult AcceptRequest(long idUser, long idGroup)
        {
            try
            {
                var userGroup = Services.UserGroup.Read(SecurityToken, idUser, idGroup);
                userGroup.DateInclusionApproval = DateTime.Now;
                Services.UserGroup.Update(SecurityToken, userGroup);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Details", "Groups", new { id = idGroup, ErrorMessage = _localizer[e.Code] });
            }
            return RedirectToAction("Details", "Groups", new { id = idGroup });
        }

        [HttpGet]
        public IActionResult RejectRequest(long idUser, long idGroup)
        {
            try
            {
                Services.UserGroup.Delete(SecurityToken, idUser, idGroup);

            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Details", "Groups", new { id = idGroup, ErrorMessage = _localizer[e.Code] });
            }
            return RedirectToAction("Details", "Groups", new { id = idGroup });
        }

        [HttpGet]
        public IActionResult LeaveGroup(long id)
        {
            try
            {
                var user = Services.User.Read(SecurityToken);
                Services.UserGroup.Delete(SecurityToken, user.Id, id);

            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Index", "Groups", new { ErrorMessage = _localizer[e.Code] });
            }
            return RedirectToAction("Index", "Groups");
        }
    }
}

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
            GroupsAndRequests result = new GroupsAndRequests();
            try
            {
                result.Groups = Services.Group.GetGroupsOfUser(SecurityToken);
                result.Requests = Services.UserGroup.GetRequestsOfUser(SecurityToken);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
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
                return BadRequest(_localizer[e.Code].Value);
            }
            return Ok();
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
                return RedirectToAction("Index", "Groups");
            }
            return View(result);
        }

        [HttpDelete]
        public IActionResult DeleteGroup(long id)
        {
            try
            {
                Services.Group.Delete(SecurityToken, id);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(_localizer[e.Code].Value);
            }
            return Ok();
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

        [HttpPost]
        public IActionResult AcceptRequest(long idUser, long idGroup)
        {
            try
            {
                Services.UserGroup.Accept(SecurityToken, idUser, idGroup);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(_localizer[e.Code].Value);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteMember(long idUser, long idGroup)
        {
            try
            {
                Services.UserGroup.Reject(SecurityToken, idUser, idGroup);

            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(_localizer[e.Code].Value);
            }
            return Ok();
        }

        [HttpDelete]
        public IActionResult LeaveGroup(long id)
        {
            try
            {
                var user = Services.User.Read(SecurityToken);
                Services.UserGroup.Reject(SecurityToken, user.Id, id);

            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(_localizer[e.Code].Value);
            }
            return Ok(Url.Action("Index", "Groups"));
        }

        [HttpPost]
        public IActionResult UpdateGroup(long idGroup, long idUser, string name)
        {
            try
            {
                var group = Services.Group.Read(SecurityToken, idGroup);
                if (idUser > 0)
                    group.IdAdmin = idUser;
                if (!string.IsNullOrWhiteSpace(name))
                    group.Name = name;
                Services.Group.Update(SecurityToken, group);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(_localizer[e.Code].Value);
            }
            return Ok();
        }
    }
}

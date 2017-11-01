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
            GroupsAndRequestsDto result = new GroupsAndRequestsDto();
            try
            {
                using (var srv = ServicesFactory.Group)
                    result = srv.GetGroupsAndRequestsOfUser(SecurityToken);
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
                using (var srv = ServicesFactory.Group)
                    srv.Create(SecurityToken, groupName);
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
                using (var srv = ServicesFactory.Group)
                    result = srv.GetGroupDetails(SecurityToken, id.Value);
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
                using (var srv = ServicesFactory.Group)
                    srv.Delete(SecurityToken, id);
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
                using (var srv = ServicesFactory.Group)
                    result = srv.GetGroupsOfUser(SecurityToken);
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
                using (var srv = ServicesFactory.Group)
                    result = srv.QueryByName(SecurityToken, name, rowQty, page);
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
                UserDto user;
                using (var srv = ServicesFactory.User)
                    user = srv.Read(SecurityToken);

                var result = new UserGroupDto
                {
                    IdGroup = id,
                    IdUser = user.Id,
                    DateInclusionRequest = DateTime.Now
                };

                using (var srv = ServicesFactory.UserGroup)
                    srv.Create(SecurityToken, result);
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
                using (var srv = ServicesFactory.UserGroup)
                    srv.Accept(SecurityToken, idUser, idGroup);
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
                using (var srv = ServicesFactory.UserGroup)
                    srv.Reject(SecurityToken, idUser, idGroup);
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
                UserDto user;
                using (var srv = ServicesFactory.User)
                    user = srv.Read(SecurityToken);

                using (var srv = ServicesFactory.UserGroup)
                    srv.Reject(SecurityToken, user.Id, id);

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
                GroupDto group;
                using (var srv = ServicesFactory.Group)
                    group = srv.Read(SecurityToken, idGroup);
                if (idUser > 0)
                    group.IdAdmin = idUser;
                if (!string.IsNullOrWhiteSpace(name))
                    group.Name = name;

                using (var srv = ServicesFactory.Group)
                   srv.Update(SecurityToken, group);
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

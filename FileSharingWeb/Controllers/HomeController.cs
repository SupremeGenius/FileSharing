﻿using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileSharingWeb.Controllers
{
    public class HomeController : BaseController
    {
        readonly ILogger<HomeController> _logger;
        readonly IStringLocalizer _localizer;

        public HomeController(ILogger<HomeController> logger, IStringLocalizerFactory factory)
        {
            _logger = logger;
            var type = typeof(Resources);
            _localizer = factory.Create(type);
        }

        [HttpGet]
        public IActionResult Index(int? id, string ErrorMessage)
        {
            FolderDetailsDto result = new FolderDetailsDto();
            ViewBag.ErrorMessage = ErrorMessage;
            try
            {
                ViewBag.FolderId = id;
                result = Services.Folder.GetFolderDetails(SecurityToken, id);
                if (id.HasValue)
                {
                    var folder = Services.Folder.Read(SecurityToken, id.Value);
                    var idRoot = folder.IdFolderRoot;
                    string path = "";
                    while (folder != null)
                    {
                        path = folder.Name + "/" + path;
                        folder = folder.IdFolderRoot.HasValue
                            ? Services.Folder.Read(SecurityToken, folder.IdFolderRoot.Value)
                            : null;
                    }
                    ViewBag.LinkFolder = idRoot.HasValue ? idRoot + "," + path : path;
                }
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                ViewBag.ErrorMessage = _localizer[e.Code];
            }
            return View(result);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SecurityToken))
                {
                    Services.User.Logout(SecurityToken);
                    Response.Cookies.Delete("SecurityToken");
                }
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
            }
            return RedirectToAction("Index", "Public");
        }

        [HttpPost]
        public IActionResult CreateFolder(string folderName, string folderRoot)
        {
            long? idFolder = null;
            string ErrorMessage = null;
            try
            {
                var folder = new FolderDto
                {
                    Name = folderName,
                };
                long idFolderRoot;
                if (long.TryParse(folderRoot, out idFolderRoot))
                {
                    folder.IdFolderRoot = idFolder = idFolderRoot;
                }
                Services.Folder.Create(SecurityToken, folder);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                ErrorMessage = _localizer[e.Code];
            }
            return Json(Url.Action("Index", "Home", new { id = idFolder, ErrorMessage = ErrorMessage }));
        }

        [HttpDelete]
        public IActionResult DeleteFolder(string folder)
        {
            long? idFolder = null;
            string ErrorMessage = null;
            try
            {
                long idFolderToDelete;
                if (long.TryParse(folder, out idFolderToDelete))
                {
                    idFolder = Services.Folder.Read(SecurityToken, idFolderToDelete)?.IdFolderRoot;
                    Services.Folder.Delete(SecurityToken, idFolderToDelete);
                }
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                ErrorMessage = _localizer[e.Code];
            }
            return Json(Url.Action("Index", "Home", new { id = idFolder, ErrorMessage = ErrorMessage }));
        }

        [HttpPost]
        public IActionResult UploadFile(long? id)
        {
            string ErrorMessage = null;
            try
            {
                if (HttpContext.Request.Form.Files != null
                    && HttpContext.Request.Form.Files.Count > 0
                    && HttpContext.Request.Form.Files[0].Length > 0)
                {
                    var uploadFile = HttpContext.Request.Form.Files[0];
                    var file = new FileDto
                    {
                        Filename = Path.GetFileName(uploadFile.FileName),
                        ContentType = uploadFile.ContentType,
                        IdFolder = id,
                        IsPublic = Convert.ToBoolean(HttpContext.Request.Form["IsPublic"])
                    };
                    long idGroup;
                    if (long.TryParse(HttpContext.Request.Form["IdGroup"], out idGroup)
                        && idGroup > 0)
                    {
                        file.IdGroup = idGroup;
                    }

                    using (var fileStream = uploadFile.OpenReadStream())
                    using (var ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        file.Content = ms.ToArray();
                    }
                    Services.File.Create(SecurityToken, file);
                }
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                ErrorMessage = _localizer[e.Code];
            }
            return Json(Url.Action("Index", "Home", new { id = id, ErrorMessage = ErrorMessage }));
        }

        [HttpPost]
        public IActionResult UpdateFile(long? id, string filename, bool isPublic, long? idGroup)
        {
            string ErrorMessage = null;
            FileDto file = new FileDto();
            try
            {
                if (id.HasValue)
                {
                    file = Services.File.Read(SecurityToken, id.Value);

                    file.Filename = filename;
                    file.IsPublic = isPublic;
                    file.IdGroup = idGroup == 0? null : idGroup;

                    Services.File.Update(SecurityToken, file);
                }
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                ErrorMessage = _localizer[e.Code];
            }
            return Json(Url.Action("Index", "Home", new { id = file.IdFolder, ErrorMessage = ErrorMessage }));
        }

        [HttpDelete]
        public IActionResult DeleteFile(long id)
        {
            string ErrorMessage = null;
            long? idFolder = null;
            try
            {
                idFolder = Services.File.Delete(SecurityToken, id);
            }
            catch (FileSharingException e)
            {
                _logger.LogError(e.Message);
                ErrorMessage = _localizer[e.Code];
            }
            return Json(Url.Action("Index", "Home", new { id = idFolder, ErrorMessage = ErrorMessage }));
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
        public IActionResult GetFile(long id)
        {
            FileDto result = null;
            try
            {
                result = Services.File.Read(SecurityToken, id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return Json(result);
        }

        [HttpGet]
        public FileResult DownloadFile(long id)
        {
            FileDto result = null;
            try
            {
                result = Services.File.Read(SecurityToken, id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return File(result.Content, result.ContentType, result.Filename);
        }
    }
}

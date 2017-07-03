using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using FileSharingWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FileSharingWeb.Controllers
{
    public class HomeController : BaseController
    {
        readonly ILogger _logger;
        readonly IStringLocalizer _localizer;

        public HomeController(ILoggerFactory loggerFactory, IStringLocalizerFactory factory)
        {
            _logger = loggerFactory.CreateLogger<PublicController>();
            var type = typeof(Resources);
            _localizer = factory.Create(type);
        }

        [HttpGet]
        public IActionResult Index(int? id, string ErrorMessage)
        {
            List<File> files = new List<File>();
            ViewBag.ErrorMessage = ErrorMessage;
            try
            {
                ViewBag.FolderRootId = id;
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
                var folders = Services.Folder.GetFoldersInFolder(SecurityToken, id);
                if (folders != null && folders.Count > 0)
                {
                    foreach(var folder in folders)
                    {
                        files.Add(new File
                        {
                            Id = folder.Id,
                            Name = folder.Name,
                            Type = FileType.Folder
                        });
                    }
                }
                var documents = Services.Document.GetDocumentsInFolder(SecurityToken, id);
                if (documents != null && documents.Count > 0)
                {
                    foreach(var document in documents)
                    {
                        files.Add(new File
                        {
                            Id = document.Id,
                            Name = document.Filename,
                            Type = FileType.Document
                        });
                    }
                }
            }
            catch (FileSharingException e)
            {
                ViewBag.ErrorMessage = _localizer[e.Code];
                _logger.LogError(2, e.Message);
            }
            return View(files);
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
                _logger.LogError(2, e.Message);
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
                if (!string.IsNullOrWhiteSpace(folderRoot))
                {
                    long idFolderRoot;
                    long.TryParse(folderRoot, out idFolderRoot);
                    idFolder = idFolderRoot;
                }
                var folder = new FolderDto
                {
                    Name = folderName,
                    IdFolderRoot = idFolder
                };
                Services.Folder.Create(SecurityToken, folder);
            }
            catch (FileSharingException e)
            {
                ErrorMessage = _localizer[e.Code];
                _logger.LogError(2, e.Message);
            }
            return RedirectToAction("Index", "Home", new { id = idFolder, ErrorMessage = ErrorMessage });
        }

        [HttpGet]
        public IActionResult UploadFile()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}

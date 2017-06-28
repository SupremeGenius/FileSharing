using FileSharing.Services.Exceptions;
using FileSharingWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FileSharingWeb.Controllers
{
    public class HomeController : BaseController
    {
        readonly ILogger _logger;

        public HomeController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PublicController>();
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<File> files = new List<File>();
            try
            {
                var folders = Services.Folder.GetFoldersInFolder(SecurityToken, null);
                if (folders != null && folders.Count > 0)
                {
                    foreach(var folder in folders)
                    {
                        files.Add(new File
                        {
                            Name = folder.Name,
                            Action = "~/folder/" + folder.Id
                        });
                    }
                }
                var documents = Services.Document.GetDocumentsInFolder(SecurityToken, null);
                if (documents != null && documents.Count > 0)
                {
                    foreach(var document in documents)
                    {
                        files.Add(new File
                        {
                            Name = document.Filename,
                            Action = string.Format("OpenDocumentModal({0})", document.Id)
                        });
                    }
                }
            }
            catch (FileSharingException e)
            {
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

        [HttpGet]
        public IActionResult CreateFolder()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult UploadFile()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}

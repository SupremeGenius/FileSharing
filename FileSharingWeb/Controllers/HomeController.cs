using FileSharing.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            try
            {
                string securityToken = "";
                Request.Cookies.TryGetValue("SecurityToken", out securityToken);
                if (!string.IsNullOrWhiteSpace(securityToken))
                {
                    Services.User.Logout(securityToken);
                    Response.Cookies.Delete("SecurityToken");
                }
            }
            catch (FileSharingException e)
            {
                _logger.LogError(2, e.Message);
            }
            return RedirectToAction("Index", "Public");
        }
    }
}

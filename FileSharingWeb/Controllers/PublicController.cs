using FileSharing.Services.Dtos;
using FileSharing.Services.Exceptions;
using FileSharingWeb.Attributes;
using FileSharingWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace FileSharingWeb.Controllers
{
    public class PublicController : BaseController
    {
		readonly ILogger _logger;
        readonly IStringLocalizer _localizer;

        public PublicController(ILoggerFactory loggerFactory, IStringLocalizerFactory factory)
		{
			_logger = loggerFactory.CreateLogger<PublicController>();
            var type = typeof(Resources);
            _localizer = factory.Create(type);
        }
		
		[HttpGet]
        [ViewLayout("_LayoutPublic")]
        public IActionResult Index()
		{
			return View("Login");
		}

        [HttpGet]
        [ViewLayout("_LayoutPublic")]
        public IActionResult Login()
        {
            return View();
        }

		[HttpPost]
        [ViewLayout("_LayoutPublic")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
		{
            if (ModelState.IsValid)
			{
				try
				{
                    var securityToken = Services.User.Login(model.Username, model.Password);
					if (!string.IsNullOrWhiteSpace(securityToken))
					{
                        var user = Services.User.Read(securityToken);
						Response.Cookies.Append("SecurityToken", securityToken);
                        Response.Cookies.Append("FirstName", user.FirstName);
                    }
                    return RedirectToAction("Index", "Home");
				}
				catch (FileSharingException e)
				{
					AddErrors(e.Code);
					_logger.LogError(2, e.Message);
				}
			}
			return View(model);
		}

        [HttpGet]
        [ViewLayout("_LayoutPublic")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ViewLayout("_LayoutPublic")]
        [ValidateAntiForgeryToken]
		public IActionResult Register(Register model)
		{
			if (ModelState.IsValid)
            {
                var user = new UserDto
				{
					Login = model.Username,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Password = model.Password
                };

				try
				{
					long result = Services.User.Register(user);
					if (result > 0)
					{
						return Redirect("Login");
					}
				}
				catch (FileSharingException e)
				{
					AddErrors(e.Message);
					_logger.LogError(1, e.Message);
				}
			}
			return View(model);
		}

		[HttpGet]
        public IActionResult Error()
		{
			return View();
		}

		#region Private methods

		void AddErrors(string error)
		{
			ModelState.AddModelError(string.Empty, _localizer[error]);
		}

		#endregion
	}
}

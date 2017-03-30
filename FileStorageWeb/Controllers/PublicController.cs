using FileStorage.Services;
using FileStorage.Services.Dtos;
using FileStorage.Services.Exceptions;
using FileStorageWeb.ViewModels.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace FileStorageWeb.Controllers
{
	public class PublicController : Controller
	{
		readonly ILogger _logger;
		UserServices userServices;

		public PublicController(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<PublicController>();
			userServices = new UserServices();
		}
		
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Login()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		[HttpPost]
		public IActionResult Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var securityToken = userServices.Login(model.UserName, model.Password);
					if (!string.IsNullOrWhiteSpace(securityToken))
					{
						Response.Cookies.Append("SecurityToken", securityToken);
					}
					return Redirect("Index"); //TODO Cambiar a home
				}
				catch (FileStorageException e)
				{
					AddErrors(e.Message);
					_logger.LogError(2, e.Message);
				}
			}
			return View();
		}

		[HttpGet]
		public IActionResult Register()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new UserDto
				{
					Login = model.UserName,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Password = model.Password
				};

				try
				{
					long result = userServices.Register(user);
					if (result > 0)
					{
						return Redirect("Login");
					}
				}
				catch (FileStorageException e)
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
			ModelState.AddModelError(string.Empty, error);
		}

		#endregion
	}
}

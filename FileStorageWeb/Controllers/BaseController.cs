using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace FileStorageWeb.Controllers
{
	public class BaseController : Controller
	{
		public string SecurityToken { get; private set; }
		public readonly ServicesFactory Services = new ServicesFactory();

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);
			SecurityToken = context.HttpContext.Request.Cookies["SecurityToken"];
			if (string.IsNullOrWhiteSpace(SecurityToken))
			{
				context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
				{
					controller = "Public",
					action = "Index"
				}));
			}
		}
	}
}

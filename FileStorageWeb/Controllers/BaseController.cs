using FileStorage.Services.Exceptions;
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

            bool tokenValid = false;

            if (!string.IsNullOrWhiteSpace(SecurityToken))
            {
                try
                {
                    Services.Session.Read(SecurityToken);
                    tokenValid = true;
                }
                catch (FileStorageException) { }

            }

			if (!tokenValid && !(context.Controller is PublicController))
			{
				context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
				{
					controller = "Public",
					action = "Index"
				}));
			}
            else if(tokenValid && context.Controller is PublicController)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Index"
                }));
            }
		}
	}
}

using FileSharing.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace FileSharingWeb.Controllers
{
    public class BaseController : Controller
	{
		public string SecurityToken { get; private set; }

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);
			SecurityToken = Request.Cookies["SecurityToken"];

            bool tokenValid = false;

            if (!string.IsNullOrWhiteSpace(SecurityToken))
            {
                try
                {
                    ServicesFactory.Session.Read(SecurityToken);
                    tokenValid = true;
                    ViewBag.FirstName = Request.Cookies["FirstName"];
                }
                catch (FileSharingException)
                {
                    Response.Cookies.Delete("SecurityToken");
                    Response.Cookies.Delete("FirstName");
                }

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

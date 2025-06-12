using CDTN.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CDTN.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = Session[ConstaintUser.ADMIN_SESSION];
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    Controller = "Login",
                    Action = "Index",
                    Area = "Admin"
                }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AtriumWebApp.Web.Base.Library
{
    public class BaseActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            BaseController controller = context.Controller as BaseController;
            controller.SetSessionVariables();
        }
    }
}

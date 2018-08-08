using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models.Budget;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AtriumWebApp.Web.Budget.Library
{
    
    public class RestrictAccessActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            byte[] byteValue = { 0 };
            context.HttpContext.Session.Set("init", byteValue);
            object actionContext;
            context.RouteData.Values.TryGetValue("action", out actionContext);

            if(actionContext != null && actionContext.ToString() == "AccessDenied")
            {
                return;
            }
            string userName = context.HttpContext.User.Identity.Name.ToLower();
            
            using (BudgetMasterIntactEntities Context = new BudgetMasterIntactEntities(BudgetMasterIntactEntities.connection))
            {
                if(Context.AllowedADUsers.Any(a => a.UserName == userName))
                {
                    return;
                }
                else
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AtriumWebApp.Web.Base.Library
{
    public class RestrictAccessWithApp : ActionFilterAttribute
    {

        public string AppCode { get; set; }
        public bool Admin { get; set; }
        public bool SysAdmin { get; set; }
        public string RedirectUrl { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            BaseController controller = context.Controller as BaseController;
            controller.SetSessionVariables();
            if(string.IsNullOrEmpty(AppCode)  && !Admin && !SysAdmin)
            {
                return;
            }
            using (SharedContext Context = new SharedContext())
            {
                //BaseController controller = context.Controller as BaseController;
                byte[] userGuid = controller.GetUserGuid();
                MasterBusinessUser user = Context.MasterBusinessUsers.First(a => a.ADObjectGuid == userGuid);

                if (user.SystemAdmin.Where(a => a.AdminFlg).Any())
                {
                    return;
                }
                bool hasAppAccess = false;
                if (!string.IsNullOrEmpty(AppCode))
                {
                    List<string> apps = AppCode.Split(',').ToList();
                    foreach (string app in apps)
                    {
                        hasAppAccess = user.UserAccess.Any(a => 
                            a.ApplicationInfo.ApplicationCode == app
                            && a.AppFlg
                            && a.ApplicationInfo.EnabledFlg);
                        if (hasAppAccess)
                        {
                            break;
                        }
                    }
                }
                if (Admin && hasAppAccess)
                {
                    hasAppAccess = user.AdminAccess.Any(a => 
                        a.AppInfo.ApplicationCode == AppCode 
                        && a.AdminFlg);
                }
                if (!hasAppAccess)
                {
                    if (string.IsNullOrEmpty(RedirectUrl))
                    {
                        context.Result = new UnauthorizedResult();
                    }
                    else
                    {
                        
                        context.Result = new RedirectResult(RedirectUrl);
                    }
                }

            }
        }
    }
}
